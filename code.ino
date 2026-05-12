#include <LiquidCrystal.h>
#include <Keypad.h>

// =====================================================
// DO AN IOT NHA THONG MINH - ARDUINO MEGA 2560
// Dieu khien qua COMPIM + Termite bang Serial1
//
// Serial1:
// TX1 = D18 -> RXD COMPIM
// RX1 = D19 -> TXD COMPIM
//
// Termite:
// Baudrate 9600
// Append CR-LF hoac LF
// =====================================================


// ================= LCD =================
// LCD 16x2 che do 4-bit
// RS -> D30
// E  -> D31
// D4 -> D32
// D5 -> D33
// D6 -> D34
// D7 -> D35
const int LCD_RS = 30;
const int LCD_E  = 31;
const int LCD_D4 = 32;
const int LCD_D5 = 33;
const int LCD_D6 = 34;
const int LCD_D7 = 35;

LiquidCrystal lcd(LCD_RS, LCD_E, LCD_D4, LCD_D5, LCD_D6, LCD_D7);


// ================= SENSOR =================
const int MQ3_PIN  = A0;  // MQ-3 OUT
const int LM35_PIN = A5;  // LM35 OUT

const int PIR_PIN  = 22;  // PIR OUT
const int RAIN_PIN = 23;  // Rain OUT


// ================= OUTPUT CANH BAO =================
const int BUZZER_PIN = 24;
const int LED_GREEN  = 25;
const int LED_YELLOW = 26;
const int LED_RED    = 27;


// ================= MOTOR CUA CHINH - L293D SO 2 =================
// Door Motor mo bang passkey/keypad
const int DOOR_EN  = 5;
const int DOOR_IN1 = 6;
const int DOOR_IN2 = 7;


// ================= MOTOR QUAT - L293D SO 1 =================
const int FAN_EN  = 8;
const int FAN_IN1 = 9;
const int FAN_IN2 = 10;


// ================= MOTOR CUA SO - L293D SO 1 =================
const int WINDOW_EN  = 11;
const int WINDOW_IN1 = 12;
const int WINDOW_IN2 = 13;


// ================= KEYPAD 4x3 =================
const byte ROWS = 4;
const byte COLS = 3;

char keys[ROWS][COLS] = {
  {'1', '2', '3'},
  {'4', '5', '6'},
  {'7', '8', '9'},
  {'*', '0', '#'}
};

// Keypad R1-R4 -> D36-D39
// Keypad C1-C3 -> D40-D42
byte rowPins[ROWS] = {36, 37, 38, 39};
byte colPins[COLS] = {40, 41, 42};

Keypad keypad = Keypad(makeKeymap(keys), rowPins, colPins, ROWS, COLS);


// ================= MAT KHAU =================
String inputPassword = "";
String adminPassword = "1234";
String userPassword  = "2580";
int wrongPasswordCount = 0;


// ================= NGUONG CANH BAO =================
int GAS_WARNING = 300;
int GAS_DANGER  = 600;

float TEMP_WARNING = 35.0;
float TEMP_DANGER  = 40.0;


// ================= TRANG THAI HE THONG =================
bool autoMode = true;

bool fanStatus = false;
bool buzzerStatus = false;
bool doorStatus = false;    // false = DONG, true = MO
bool windowStatus = false;  // false = DONG, true = MO

String systemLevel = "AN TOAN";
String lastMessage = "HE THONG AN TOAN";


// ================= TIMER =================
unsigned long lastSendTime = 0;
unsigned long lastLcdTime = 0;

const unsigned long SEND_INTERVAL = 1000;
const unsigned long LCD_INTERVAL  = 800;


// ================= SETUP =================
void setup() {
  Serial.begin(9600);   // debug neu dung Serial Monitor
  Serial1.begin(9600);  // COMPIM / Termite

  lcd.begin(16, 2);
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("NHA THONG MINH");
  lcd.setCursor(0, 1);
  lcd.print("DANG KHOI DONG");
  delay(1500);

  pinMode(PIR_PIN, INPUT);
  pinMode(RAIN_PIN, INPUT);

  pinMode(BUZZER_PIN, OUTPUT);
  pinMode(LED_GREEN, OUTPUT);
  pinMode(LED_YELLOW, OUTPUT);
  pinMode(LED_RED, OUTPUT);

  pinMode(DOOR_EN, OUTPUT);
  pinMode(DOOR_IN1, OUTPUT);
  pinMode(DOOR_IN2, OUTPUT);

  pinMode(FAN_EN, OUTPUT);
  pinMode(FAN_IN1, OUTPUT);
  pinMode(FAN_IN2, OUTPUT);

  pinMode(WINDOW_EN, OUTPUT);
  pinMode(WINDOW_IN1, OUTPUT);
  pinMode(WINDOW_IN2, OUTPUT);

  stopDoor();
  fanOff();
  stopWindow();
  buzzerOff();
  setLedSafe();

  sendLine("HE_THONG=NHA_THONG_MINH;TRANG_THAI=SAN_SANG");
  sendLine("GO_LENH_HELP_DE_XEM_DANH_SACH_LENH");

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("HE THONG");
  lcd.setCursor(0, 1);
  lcd.print("SAN SANG");
  delay(1000);
}


// ================= LOOP =================
void loop() {
  int gasValue = analogRead(MQ3_PIN);
  float temperature = readTemperature();
  int pirStatus = digitalRead(PIR_PIN);
  int rainStatus = digitalRead(RAIN_PIN);

  handleKeypad();
  readSerialCommand();

  if (autoMode) {
    autoControl(gasValue, temperature, pirStatus, rainStatus);
  }

  updateLcd(gasValue, temperature, pirStatus, rainStatus);

  if (millis() - lastSendTime >= SEND_INTERVAL) {
    lastSendTime = millis();
    sendDataToTermite(gasValue, temperature, pirStatus, rainStatus);
  }
}


// ================= HAM GUI SERIAL =================
void sendLine(String text) {
  Serial.println(text);
  Serial1.println(text);
}


// ================= DOC NHIET DO LM35 =================
float readTemperature() {
  int adc = analogRead(LM35_PIN);
  float voltage = adc * 5.0 / 1023.0;
  float tempC = voltage * 100.0;
  return tempC;
}


// ================= TU DONG DIEU KHIEN =================
void autoControl(int gasValue, float temperature, int pirStatus, int rainStatus) {
  bool gasWarning = gasValue >= GAS_WARNING;
  bool gasDanger  = gasValue >= GAS_DANGER;

  bool tempWarning = temperature >= TEMP_WARNING;
  bool tempDanger  = temperature >= TEMP_DANGER;

  bool personDetected = pirStatus == HIGH;
  bool rainDetected   = rainStatus == HIGH;

  if (gasDanger || tempDanger || (gasValue >= 400 && personDetected)) {
    systemLevel = "NGUY HIEM";

    if (gasDanger) {
      lastMessage = "KHI CON CAO";
    }
    else if (tempDanger) {
      lastMessage = "NHIET DO RAT CAO";
    }
    else {
      lastMessage = "KHI+CO NGUOI";
    }

    setLedDanger();
    buzzerOn();
    fanOn();

    if (!rainDetected) {
      openWindow();
    }
    else {
      closeWindow();
      lastMessage = "KHI CAO+CO MUA";
    }
  }
  else if (gasWarning || tempWarning || rainDetected) {
    systemLevel = "CANH BAO";

    if (gasWarning) {
      lastMessage = "CANH BAO KHI";
    }
    else if (tempWarning) {
      lastMessage = "NHIET DO CAO";
    }
    else {
      lastMessage = "CO MUA";
    }

    setLedWarning();
    buzzerOff();

    if (tempWarning || gasWarning) {
      fanOn();
    }
    else {
      fanOff();
    }

    if (rainDetected) {
      closeWindow();
    }
  }
  else {
    systemLevel = "AN TOAN";
    lastMessage = "HE THONG AN TOAN";

    setLedSafe();
    buzzerOff();
    fanOff();
    stopWindow();
  }

  if (personDetected && systemLevel == "AN TOAN") {
    lastMessage = "CO NGUOI";
  }
}


// ================= KEYPAD VAT LY =================
void handleKeypad() {
  char key = keypad.getKey();

  if (!key) return;

  sendLine("KEYPAD=PHIM_" + String(key));

  if (key == '*') {
    inputPassword = "";
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("XOA MAT KHAU");

    sendLine("KEYPAD=XOA_MAT_KHAU");
    delay(300);
    return;
  }

  if (key == '#') {
    checkPassword();
    inputPassword = "";
    return;
  }

  if (inputPassword.length() < 8) {
    inputPassword += key;

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("NHAP MK:");
    lcd.setCursor(0, 1);

    for (int i = 0; i < inputPassword.length(); i++) {
      lcd.print("*");
    }

    String mask = "";
    for (int i = 0; i < inputPassword.length(); i++) {
      mask += "*";
    }

    sendLine("KEYPAD=DANG_NHAP;" + mask);
  }
}


// ================= KIEM TRA MAT KHAU =================
void checkPassword() {
  if (inputPassword == adminPassword || inputPassword == userPassword) {
    wrongPasswordCount = 0;

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("DUNG MAT KHAU");
    lcd.setCursor(0, 1);
    lcd.print("DANG MO CUA");

    sendLine("TRUY_CAP=THANH_CONG;CUA=DANG_MO");

    openDoor();
    delay(2000);
    stopDoor();

    doorStatus = true;

    delay(5000);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("DANG DONG CUA");

    closeDoor();
    delay(2000);
    stopDoor();

    doorStatus = false;

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("CUA DA KHOA");

    sendLine("TRUY_CAP=THANH_CONG;CUA=DA_KHOA");

    delay(800);
  }
  else {
    wrongPasswordCount++;

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("SAI MAT KHAU");
    lcd.setCursor(0, 1);
    lcd.print("SO LAN:");
    lcd.print(wrongPasswordCount);

    beepShort();

    sendLine("TRUY_CAP=TU_CHOI;SO_LAN=" + String(wrongPasswordCount));

    if (wrongPasswordCount >= 3) {
      systemLevel = "NGUY HIEM";
      lastMessage = "CANH BAO AN NINH";

      setLedDanger();
      buzzerOn();

      sendLine("CANH_BAO=AN_NINH;MUC=NGUY_HIEM;NOI_DUNG=SAI_MAT_KHAU_3_LAN");

      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("CANH BAO");
      lcd.setCursor(0, 1);
      lcd.print("SAI MK 3 LAN");

      delay(3000);

      wrongPasswordCount = 0;
      buzzerOff();
    }
  }
}


// ================= NHAN LENH TU TERMITE / WINFORM =================
// Nhan duoc ca LF, CR, CR-LF
void readSerialCommand() {
  static String cmd1 = "";
  static String cmd0 = "";

  while (Serial1.available()) {
    char c = Serial1.read();

    if (c == '\n' || c == '\r') {
      cmd1.trim();
      if (cmd1.length() > 0) {
        handleCommand(cmd1);
        cmd1 = "";
      }
    }
    else {
      cmd1 += c;
    }
  }

  while (Serial.available()) {
    char c = Serial.read();

    if (c == '\n' || c == '\r') {
      cmd0.trim();
      if (cmd0.length() > 0) {
        handleCommand(cmd0);
        cmd0 = "";
      }
    }
    else {
      cmd0 += c;
    }
  }
}


void handleCommand(String cmd) {
  cmd.trim();

  if (cmd.length() == 0) return;

  // Cho phep nhap mat khau tu Termite:
  // Vi du go: 1234# hoac 2580#
  if (cmd.endsWith("#")) {
    inputPassword = cmd;
    inputPassword.replace("#", "");

    sendLine("TERMITE=NHAP_MAT_KHAU;" + inputPassword);

    checkPassword();
    inputPassword = "";
    return;
  }

  cmd.toUpperCase();

  if (cmd == "HELP") {
    printHelp();
  }
  else if (cmd == "STATUS") {
    int gasValue = analogRead(MQ3_PIN);
    float temperature = readTemperature();
    int pirStatus = digitalRead(PIR_PIN);
    int rainStatus = digitalRead(RAIN_PIN);
    sendDataToTermite(gasValue, temperature, pirStatus, rainStatus);
  }
  else if (cmd == "MODE_AUTO") {
    autoMode = true;
    sendLine("CHE_DO=TU_DONG;KET_QUA=OK");
  }
  else if (cmd == "MODE_MANUAL") {
    autoMode = false;
    sendLine("CHE_DO=THU_CONG;KET_QUA=OK");
  }

  else if (cmd == "FAN_ON") {
    fanOn();
    sendLine("LENH=BAT_QUAT;KET_QUA=OK");
  }
  else if (cmd == "FAN_OFF") {
    fanOff();
    sendLine("LENH=TAT_QUAT;KET_QUA=OK");
  }

  else if (cmd == "WINDOW_OPEN") {
    openWindow();
    delay(2000);
    stopWindow();
    sendLine("LENH=MO_CUA_SO;KET_QUA=OK");
  }
  else if (cmd == "WINDOW_CLOSE") {
    closeWindow();
    delay(2000);
    stopWindow();
    sendLine("LENH=DONG_CUA_SO;KET_QUA=OK");
  }
  else if (cmd == "WINDOW_STOP") {
    stopWindow();
    sendLine("LENH=DUNG_CUA_SO;KET_QUA=OK");
  }

  else if (cmd == "DOOR_OPEN") {
    openDoor();
    delay(2000);
    stopDoor();
    doorStatus = true;
    sendLine("LENH=MO_CUA_CHINH;KET_QUA=OK");
  }
  else if (cmd == "DOOR_CLOSE") {
    closeDoor();
    delay(2000);
    stopDoor();
    doorStatus = false;
    sendLine("LENH=DONG_CUA_CHINH;KET_QUA=OK");
  }
  else if (cmd == "DOOR_STOP") {
    stopDoor();
    sendLine("LENH=DUNG_CUA_CHINH;KET_QUA=OK");
  }

  else if (cmd == "BUZZER_ON") {
    buzzerOn();
    sendLine("LENH=BAT_COI;KET_QUA=OK");
  }
  else if (cmd == "BUZZER_OFF") {
    buzzerOff();
    sendLine("LENH=TAT_COI;KET_QUA=OK");
  }

  else if (cmd == "LED_SAFE") {
    setLedSafe();
    systemLevel = "AN TOAN";
    lastMessage = "DIEU KHIEN LED SAFE";
    sendLine("LENH=LED_AN_TOAN;KET_QUA=OK");
  }
  else if (cmd == "LED_WARNING") {
    setLedWarning();
    systemLevel = "CANH BAO";
    lastMessage = "DIEU KHIEN LED WARNING";
    sendLine("LENH=LED_CANH_BAO;KET_QUA=OK");
  }
  else if (cmd == "LED_DANGER") {
    setLedDanger();
    systemLevel = "NGUY HIEM";
    lastMessage = "DIEU KHIEN LED DANGER";
    sendLine("LENH=LED_NGUY_HIEM;KET_QUA=OK");
  }

  else if (cmd == "RESET_ALARM") {
    buzzerOff();
    setLedSafe();
    systemLevel = "AN TOAN";
    lastMessage = "DA RESET CANH BAO";
    sendLine("LENH=RESET_CANH_BAO;KET_QUA=OK");
  }

  else {
    sendLine("LOI=LENH_KHONG_HOP_LE;" + cmd);
    sendLine("GO_LENH=HELP_DE_XEM_DANH_SACH_LENH");
  }
}


// ================= HELP =================
void printHelp() {
  sendLine("===== DANH SACH LENH =====");
  sendLine("MODE_AUTO");
  sendLine("MODE_MANUAL");
  sendLine("FAN_ON");
  sendLine("FAN_OFF");
  sendLine("WINDOW_OPEN");
  sendLine("WINDOW_CLOSE");
  sendLine("WINDOW_STOP");
  sendLine("DOOR_OPEN");
  sendLine("DOOR_CLOSE");
  sendLine("DOOR_STOP");
  sendLine("BUZZER_ON");
  sendLine("BUZZER_OFF");
  sendLine("LED_SAFE");
  sendLine("LED_WARNING");
  sendLine("LED_DANGER");
  sendLine("RESET_ALARM");
  sendLine("STATUS");
  sendLine("HELP");
  sendLine("Nhap mat khau tu Termite: 1234# hoac 2580#");
  sendLine("==========================");
}


// ================= MOTOR CONTROL =================
void openDoor() {
  digitalWrite(DOOR_EN, HIGH);
  digitalWrite(DOOR_IN1, HIGH);
  digitalWrite(DOOR_IN2, LOW);
  doorStatus = true;
}

void closeDoor() {
  digitalWrite(DOOR_EN, HIGH);
  digitalWrite(DOOR_IN1, LOW);
  digitalWrite(DOOR_IN2, HIGH);
  doorStatus = false;
}

void stopDoor() {
  digitalWrite(DOOR_EN, LOW);
  digitalWrite(DOOR_IN1, LOW);
  digitalWrite(DOOR_IN2, LOW);
}


void fanOn() {
  digitalWrite(FAN_EN, HIGH);
  digitalWrite(FAN_IN1, HIGH);
  digitalWrite(FAN_IN2, LOW);
  fanStatus = true;
}

void fanOff() {
  digitalWrite(FAN_EN, LOW);
  digitalWrite(FAN_IN1, LOW);
  digitalWrite(FAN_IN2, LOW);
  fanStatus = false;
}


void openWindow() {
  digitalWrite(WINDOW_EN, HIGH);
  digitalWrite(WINDOW_IN1, HIGH);
  digitalWrite(WINDOW_IN2, LOW);
  windowStatus = true;
}

void closeWindow() {
  digitalWrite(WINDOW_EN, HIGH);
  digitalWrite(WINDOW_IN1, LOW);
  digitalWrite(WINDOW_IN2, HIGH);
  windowStatus = false;
}

void stopWindow() {
  digitalWrite(WINDOW_EN, LOW);
  digitalWrite(WINDOW_IN1, LOW);
  digitalWrite(WINDOW_IN2, LOW);
}


// ================= LED & BUZZER =================
void setLedSafe() {
  digitalWrite(LED_GREEN, HIGH);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, LOW);
}

void setLedWarning() {
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, HIGH);
  digitalWrite(LED_RED, LOW);
}

void setLedDanger() {
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, HIGH);
}

void buzzerOn() {
  digitalWrite(BUZZER_PIN, HIGH);
  buzzerStatus = true;
}

void buzzerOff() {
  digitalWrite(BUZZER_PIN, LOW);
  buzzerStatus = false;
}

void beepShort() {
  digitalWrite(BUZZER_PIN, HIGH);
  delay(150);
  digitalWrite(BUZZER_PIN, LOW);
  delay(150);
}


// ================= LCD =================
void updateLcd(int gasValue, float temperature, int pirStatus, int rainStatus) {
  if (millis() - lastLcdTime < LCD_INTERVAL) return;

  lastLcdTime = millis();

  lcd.clear();

  lcd.setCursor(0, 0);
  lcd.print(systemLevel);
  lcd.print(" ");
  lcd.print((int)temperature);
  lcd.print("C");

  lcd.setCursor(0, 1);

  if (systemLevel == "AN TOAN") {
    lcd.print("KHI:");
    lcd.print(gasValue);

    if (pirStatus == HIGH) {
      lcd.print(" NGUOI");
    }
  }
  else {
    lcd.print(lastMessage);
  }
}


// ================= GUI DATA SANG TERMITE / WINFORM =================
void sendDataToTermite(int gasValue, float temperature, int pirStatus, int rainStatus) {
  String data = "";

  data += "KHI=";
  data += gasValue;

  data += ";NHIET_DO=";
  data += String(temperature, 1);

  data += ";CO_NGUOI=";
  data += pirStatus;

  data += ";MUA=";
  data += rainStatus;

  data += ";QUAT=";
  data += fanStatus ? "BAT" : "TAT";

  data += ";CUA_SO=";
  data += windowStatus ? "MO" : "DONG";

  data += ";CUA_CHINH=";
  data += doorStatus ? "MO" : "DONG";

  data += ";COI_BAO=";
  data += buzzerStatus ? "BAT" : "TAT";

  data += ";CHE_DO=";
  data += autoMode ? "TU_DONG" : "THU_CONG";

  data += ";MUC_CANH_BAO=";
  data += systemLevel;

  data += ";THONG_BAO=";
  data += lastMessage;

  sendLine(data);
}