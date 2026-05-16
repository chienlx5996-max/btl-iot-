#include <LiquidCrystal.h>
#include <Keypad.h>

// ================= CẤU HÌNH LCD 16X2 =================
const int LCD_RS = 30, LCD_E = 31, LCD_D4 = 32, LCD_D5 = 33, LCD_D6 = 34, LCD_D7 = 35;
LiquidCrystal lcd(LCD_RS, LCD_E, LCD_D4, LCD_D5, LCD_D6, LCD_D7);

// ================= CHÂN CẢM BIẾN (SENSORS) =================
const int MQ3_PIN   = A0; // BIẾN TRỞ NỐI VÀO ĐÂY
const int LM35_PIN  = A5; 
const int PIR_PIN   = 22; 
const int RAIN_PIN  = 23; 

// ================= CHÂN ĐIỀU KHIỂN =================
const int BUZZER_PIN = 24;
const int LED_GREEN  = 25, LED_YELLOW = 26, LED_RED = 27;

// ================= ĐỘNG CƠ L293D =================
const int DOOR_EN = 5, DOOR_IN1 = 6, DOOR_IN2 = 7;
const int FAN_EN = 8, FAN_IN1 = 9, FAN_IN2 = 10;
const int WINDOW_EN = 11, WINDOW_IN1 = 12, WINDOW_IN2 = 13;

// ================= BÀN PHÍM (KEYPAD) =================
const byte ROWS = 4, COLS = 3;
char keys[ROWS][COLS] = {{'1','2','3'}, {'4','5','6'}, {'7','8','9'}, {'*','0','#'}};
byte rowPins[ROWS] = {36, 37, 38, 39};
byte colPins[COLS] = {40, 41, 42};
Keypad keypad = Keypad(makeKeymap(keys), rowPins, colPins, ROWS, COLS);

// ================= BIẾN HỆ THỐNG =================
String inputPassword = "";
const String adminPassword = "1234";
int wrongPassCount = 0;

bool autoMode = true;
bool fanStatus = false, doorStatus = false, windowStatus = false, buzzerStatus = false;
String systemLevel = "AN TOAN", lastMessage = "HE THONG OK";
String tempMsg = ""; 

unsigned long lastSend = 0, lastLcd = 0, msgTimer = 0;
unsigned long doorTimer = 0, windowTimer = 0;
bool isDoorMoving = false, isWindowMoving = false;

const unsigned long MOTOR_TIME = 2000; 
const unsigned long MSG_HOLD   = 2000; // Giữ tin nhắn 2 giây để kịp đọc

// ================= SETUP =================
void setup() {
  Serial.begin(9600);   
  Serial1.begin(9600);  

  lcd.begin(16, 2);
  lcd.print("DANG KHOI DONG");

  pinMode(PIR_PIN, INPUT); pinMode(RAIN_PIN, INPUT);
  pinMode(BUZZER_PIN, OUTPUT);
  pinMode(LED_GREEN, OUTPUT); pinMode(LED_YELLOW, OUTPUT); pinMode(LED_RED, OUTPUT);
  
  pinMode(DOOR_EN, OUTPUT); pinMode(DOOR_IN1, OUTPUT); pinMode(DOOR_IN2, OUTPUT);
  pinMode(FAN_EN, OUTPUT); pinMode(FAN_IN1, OUTPUT); pinMode(FAN_IN2, OUTPUT);
  pinMode(WINDOW_EN, OUTPUT); pinMode(WINDOW_IN1, OUTPUT); pinMode(WINDOW_IN2, OUTPUT);

  stopAll();
  setLedSafe();
  delay(1000);
  lcd.clear();
}

// ================= LOOP =================
void loop() {
  // Đọc giá trị từ biến trở giả lập Gas
  int gas = analogRead(MQ3_PIN);
  float t = (analogRead(LM35_PIN) * 5.0 / 1023.0) * 100.0;
  int pir = digitalRead(PIR_PIN);
  int rain = digitalRead(RAIN_PIN);

  handleKeypad();
  readSerial();
  updateMotors();

  if (autoMode) {
    autoLogic(gas, t, pir, rain);
  }

  updateDisplay(gas, t);
  
  if (millis() - lastSend >= 1500) {
    lastSend = millis();
    sendData(gas, t, pir, rain);
  }
}

// ================= LOGIC TỰ ĐỘNG =================
void autoLogic(int gas, float temp, int pir, int rain) {
  bool needsFan = (temp > 35.0 || gas > 300);
  bool isDanger = (temp > 40.0 || gas > 600);
  bool isRaining = (rain == HIGH);

  if (isDanger) {
    systemLevel = "NGUY HIEM";
    lastMessage = (gas > 600) ? "KHI GAS CAO!" : "HOA HOAN!";
    setLedDanger(); buzzerOn(); fanOn();
    if (!isRaining && !isWindowMoving) startWindow(true); 
  } 
  else if (needsFan || isRaining) {
    systemLevel = "CANH BAO";
    if (gas > 300) lastMessage = "CANH BAO GAS";
    else if (temp > 35.0) lastMessage = "NHIET DO CAO";
    else lastMessage = "CO MUA - DONG CS";
    
    setLedWarning(); buzzerOff();
    if (needsFan) fanOn(); else fanOff();
    // Chỉ đóng cửa sổ nếu đang mưa và chưa quay
    if (isRaining && !isWindowMoving) startWindow(false);
    // Tránh bật/tắt liên tục - nếu window đang quay thì không can thiệp
  } 
  else {
    systemLevel = "AN TOAN";
    lastMessage = (pir == HIGH) ? "CO NGUOI" : "HE THONG OK";
    setLedSafe(); buzzerOff();
    if (!isDoorMoving && !isWindowMoving) fanOff(); 
  }
}

// ================= GIAO DIỆN LCD =================
void updateDisplay(int gas, float t) {
  if (millis() - lastLcd < 400) return; // Cập nhật nhanh hơn để thấy biến trở nhảy số
  lastLcd = millis();

  lcd.setCursor(0, 0);
  lcd.print(systemLevel); lcd.print(" "); lcd.print((int)t); lcd.print("C  ");

  lcd.setCursor(0, 1);
  // Ưu tiên hiện tin nhắn sự kiện (nhập pass, face id) trong 2s
  if (millis() - msgTimer < MSG_HOLD) {
    lcd.print(tempMsg); lcd.print("          ");
  } 
  else if (inputPassword.length() > 0) {
    lcd.print("PASS: ");
    for(int i=0; i<inputPassword.length(); i++) lcd.print("*");
    lcd.print("    ");
  } 
  else {
    // Hiện giá trị gas thực tế khi xoay biến trở
    lcd.print(lastMessage); 
    lcd.setCursor(12, 1);
    lcd.print(gas); lcd.print("  ");
  }
}

// ================= CÁC HÀM ĐIỀU KHIỂN =================
void updateMotors() {
  if (isDoorMoving && (millis() - doorTimer >= MOTOR_TIME)) {
    digitalWrite(DOOR_EN, LOW); isDoorMoving = false;
  }
  if (isWindowMoving && (millis() - windowTimer >= MOTOR_TIME)) {
    digitalWrite(WINDOW_EN, LOW); isWindowMoving = false;
  }
}

void startDoor(bool open) {
  // Nếu đang quay, dừng trước
  if (isDoorMoving) {
    digitalWrite(DOOR_EN, LOW); 
    isDoorMoving = false;
    delay(100);  // Chờ motor dừng
  }
  
  digitalWrite(DOOR_EN, HIGH);
  digitalWrite(DOOR_IN1, open ? HIGH : LOW);
  digitalWrite(DOOR_IN2, open ? LOW : HIGH);
  doorStatus = open; 
  doorTimer = millis(); 
  isDoorMoving = true;
}

void stopDoor() {
  if (isDoorMoving) {
    digitalWrite(DOOR_EN, LOW); 
    isDoorMoving = false;
  }
}

void startWindow(bool open) {
  // Nếu đang quay, dừng trước
  if (isWindowMoving) {
    digitalWrite(WINDOW_EN, LOW); 
    isWindowMoving = false;
    delay(100);  // Chờ motor dừng
  }
  
  digitalWrite(WINDOW_EN, HIGH);
  digitalWrite(WINDOW_IN1, open ? HIGH : LOW);
  digitalWrite(WINDOW_IN2, open ? LOW : HIGH);
  windowStatus = open; 
  windowTimer = millis(); 
  isWindowMoving = true;
}

void stopWindow() {
  if (isWindowMoving) {
    digitalWrite(WINDOW_EN, LOW); 
    isWindowMoving = false;
  }
}

void handleKeypad() {
  char key = keypad.getKey();
  if (!key) return;
  if (key == '*') { inputPassword = ""; tempMsg = "DA XOA"; msgTimer = millis(); }
  else if (key == '#') {
    if (inputPassword == adminPassword) { startDoor(true); tempMsg = "MO CUA"; }
    else { tempMsg = "SAI MK!"; wrongPassCount++; if(wrongPassCount>=3) buzzerOn(); }
    inputPassword = ""; msgTimer = millis();
  } else if (inputPassword.length() < 4) inputPassword += key;
}

void readSerial() {
  if (Serial1.available()) {
    String cmd = Serial1.readStringUntil('\n');
    cmd.trim(); cmd.toUpperCase();
    
    if (cmd == "FACE_OK" || cmd == "DOOR_OPEN") { 
      startDoor(true); 
      tempMsg = "FACE ID OK"; 
      msgTimer = millis();
      Serial1.println("DOOR_OPEN|OK");  // Gửi response
      Serial.println("DOOR_OPEN|OK");
    }
    else if (cmd == "FAN_ON") { 
      fanOn();
      Serial1.println("FAN_ON|OK");  // Gửi response
      Serial.println("FAN_ON|OK");
    }
    else if (cmd == "FAN_OFF") { 
      fanOff();
      Serial1.println("FAN_OFF|OK");  // Gửi response
      Serial.println("FAN_OFF|OK");
    }
    else if (cmd == "DOOR_CLOSE") { 
      startDoor(false);
      Serial1.println("DOOR_CLOSE|OK");
      Serial.println("DOOR_CLOSE|OK");
    }
    else if (cmd == "WINDOW_OPEN") { 
      startWindow(true);
      Serial1.println("WINDOW_OPEN|OK");
      Serial.println("WINDOW_OPEN|OK");
    }
    else if (cmd == "WINDOW_CLOSE") { 
      startWindow(false);
      Serial1.println("WINDOW_CLOSE|OK");
      Serial.println("WINDOW_CLOSE|OK");
    }
    else if (cmd == "BUZZER_ON") { 
      buzzerOn();
      Serial1.println("BUZZER_ON|OK");
      Serial.println("BUZZER_ON|OK");
    }
    else if (cmd == "BUZZER_OFF") { 
      buzzerOff();
      Serial1.println("BUZZER_OFF|OK");
      Serial.println("BUZZER_OFF|OK");
    }
    else if (cmd == "MODE_AUTO") { 
      autoMode = true;
      Serial1.println("MODE_AUTO|OK");
      Serial.println("MODE_AUTO|OK");
    }
    else if (cmd == "MODE_MANUAL") { 
      autoMode = false;
      Serial1.println("MODE_MANUAL|OK");
      Serial.println("MODE_MANUAL|OK");
    }
  }
}

void fanOn() { 
  if (!fanStatus) {
    digitalWrite(FAN_IN1, HIGH);   // Set direction
    digitalWrite(FAN_IN2, LOW);
    analogWrite(FAN_EN, 255);      // PWM 100% power (0-255)
    fanStatus = true;
  }
}

void fanOff() { 
  if (fanStatus) {
    analogWrite(FAN_EN, 0);        // PWM 0% power
    fanStatus = false;
  }
}
void buzzerOn() { digitalWrite(BUZZER_PIN, HIGH); buzzerStatus = true; }
void buzzerOff() { digitalWrite(BUZZER_PIN, LOW); buzzerStatus = false; }
void setLedSafe() { digitalWrite(LED_GREEN, HIGH); digitalWrite(LED_YELLOW, LOW); digitalWrite(LED_RED, LOW); }
void setLedWarning() { digitalWrite(LED_GREEN, LOW); digitalWrite(LED_YELLOW, HIGH); digitalWrite(LED_RED, LOW); }
void setLedDanger() { digitalWrite(LED_GREEN, LOW); digitalWrite(LED_YELLOW, LOW); digitalWrite(LED_RED, HIGH); }
void stopAll() { digitalWrite(DOOR_EN, LOW); digitalWrite(FAN_EN, LOW); digitalWrite(WINDOW_EN, LOW); buzzerOff(); }
void sendLine(String t) { Serial.println(t); Serial1.println(t); }
void sendData(int g, float t, int p, int r) {
  // Gửi sensor data + trạng thái thiết bị
  String d = "STAT|GAS:" + String(g) 
           + "|TEMP:" + String(t,1) 
           + "|PIR:" + String(p)
           + "|RAIN:" + String(r)
           + "|QUAT:" + (fanStatus ? "BAT" : "TAT")
           + "|CUA_CHINH:" + (doorStatus ? "MO" : "DONG")
           + "|CUA_SO:" + (windowStatus ? "MO" : "DONG")
           + "|COI_BAO:" + (buzzerStatus ? "BAT" : "TAT")
           + "|MODE:" + (autoMode ? "A" : "M");
  Serial1.println(d);
}