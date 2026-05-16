#include <LiquidCrystal.h>
#include <Keypad.h>

// ================= CẤU HÌNH LCD 16X2 =================
const int LCD_RS = 30, LCD_E = 31, LCD_D4 = 32, LCD_D5 = 33, LCD_D6 = 34, LCD_D7 = 35;
LiquidCrystal lcd(LCD_RS, LCD_E, LCD_D4, LCD_D5, LCD_D6, LCD_D7);

// ================= CẢM BIẾN =================
const int MQ3_PIN   = A0;
const int LM35_PIN  = A5;
const int PIR_PIN   = 22;
const int RAIN_PIN  = 23;

// ================= ĐIỀU KHIỂN =================
const int BUZZER_PIN = 24;
const int LED_GREEN  = 25, LED_YELLOW = 26, LED_RED = 27;

// ================= MOTOR L293D =================
const int DOOR_EN   = 5, DOOR_IN1 = 6, DOOR_IN2 = 7;
const int FAN_EN    = 8, FAN_IN1 = 9, FAN_IN2 = 10;
const int WINDOW_EN = 11, WINDOW_IN1 = 12, WINDOW_IN2 = 13;

// ================= NÚT DỪNG MOTOR =================
const int BTN_DOOR   = 44;
const int BTN_FAN    = 45;
const int BTN_WINDOW = 46;

// ================= BÀN PHÍM =================
const byte ROWS = 4, COLS = 3;
char keys[ROWS][COLS] = {{'1','2','3'},{'4','5','6'},{'7','8','9'},{'*','0','#'}};
byte rowPins[ROWS] = {36,37,38,39};
byte colPins[COLS] = {40,41,42};
Keypad keypad = Keypad(makeKeymap(keys), rowPins, colPins, ROWS, COLS);

// ================= BIẾN HỆ THỐNG =================
String inputPassword = "";
const String adminPassword = "1234";

bool autoMode = true;
bool fanStatus=false, doorStatus=false, windowStatus=false, buzzerStatus=false;
bool isDoorMoving=false, isWindowMoving=false;

String systemLevel="AN TOAN";
String lastMessage="HE THONG OK";

String tempMsg = "";
unsigned long msgTimer=0;
unsigned long doorTimer=0, windowTimer=0;
const unsigned long MOTOR_TIME=2000;

// ================= SETUP =================
void setup(){
  Serial.begin(9600);
  Serial1.begin(9600);

  lcd.begin(16,2);
  lcd.print("DANG KHOI DONG");

  pinMode(PIR_PIN, INPUT);
  pinMode(RAIN_PIN, INPUT);
  pinMode(BUZZER_PIN, OUTPUT);
  pinMode(LED_GREEN, OUTPUT); pinMode(LED_YELLOW, OUTPUT); pinMode(LED_RED, OUTPUT);

  pinMode(DOOR_EN, OUTPUT); pinMode(DOOR_IN1, OUTPUT); pinMode(DOOR_IN2, OUTPUT);
  pinMode(FAN_EN, OUTPUT); pinMode(FAN_IN1, OUTPUT); pinMode(FAN_IN2, OUTPUT);
  pinMode(WINDOW_EN, OUTPUT); pinMode(WINDOW_IN1, OUTPUT); pinMode(WINDOW_IN2, OUTPUT);

  pinMode(BTN_DOOR, INPUT_PULLUP);
  pinMode(BTN_FAN, INPUT_PULLUP);
  pinMode(BTN_WINDOW, INPUT_PULLUP);

  stopAll();
  setLedSafe();
  delay(1000);
  lcd.clear();
}

// ================= LOOP =================
void loop(){
  int gas = analogRead(MQ3_PIN);
  float temp = analogRead(LM35_PIN)*5.0/1023.0*100.0;
  int pir = digitalRead(PIR_PIN);
  int rain = digitalRead(RAIN_PIN);

  handleKeypad();
  readSerial();
  updateMotors();
  checkStopButtons();

  if(autoMode) autoLogic(gas,temp,pir,rain);
  updateDisplay(gas,temp);

  delay(50); // phản hồi nút stop nhanh
}

// ================= LOGIC TỰ ĐỘNG =================
void autoLogic(int gas,float temp,int pir,int rain){
  bool needsFan=(temp>35.0 || gas>300);
  bool isDanger=(temp>40.0 || gas>600);
  bool isRaining=(rain==HIGH);

  if(isDanger){
    systemLevel="NGUY HIEM";
    lastMessage=(gas>600)?"KHI GAS CAO!":"HOA HOAN!";
    setLedDanger();
    buzzerOn();
    fanOn(); 
    if(!isRaining) startWindow(true);
  }
  else if(needsFan || isRaining){
    systemLevel="CANH BAO";
    lastMessage=(needsFan)?"NHIET DO/GAS CAO":"CO MUA - DONG CS";
    setLedWarning();
    buzzerOff();
    if(needsFan) fanOn(); else fanOff();
    if(isRaining) startWindow(false);
  }
  else{
    systemLevel="AN TOAN";
    lastMessage=(pir==HIGH)?"CO NGUOI":"HE THONG OK";
    setLedSafe();
    buzzerOff();
    fanOff(); // dừng hẳn quạt
    stopDoor();
    stopWindow();
  }
}

// ================= HIỂN THỊ LCD =================
void updateDisplay(int gas,float temp){
  lcd.setCursor(0,0);
  lcd.print(systemLevel); lcd.print(" "); lcd.print((int)temp); lcd.print("C  ");
  lcd.setCursor(0,1);
  lcd.print(lastMessage); lcd.print("    ");
  lcd.setCursor(12,1);
  lcd.print(gas); lcd.print("  ");
}

// ================= MOTOR =================
void startDoor(bool open){
  digitalWrite(DOOR_EN,HIGH);
  digitalWrite(DOOR_IN1,open?HIGH:LOW);
  digitalWrite(DOOR_IN2,open?LOW:HIGH);
  doorStatus=open;
  isDoorMoving=true;
  doorTimer=millis();
}
void stopDoor(){
  digitalWrite(DOOR_EN,LOW);
  digitalWrite(DOOR_IN1,LOW);
  digitalWrite(DOOR_IN2,LOW);
  isDoorMoving=false;
  doorStatus=false;
}

void startWindow(bool open){
  digitalWrite(WINDOW_EN,HIGH);
  digitalWrite(WINDOW_IN1,open?HIGH:LOW);
  digitalWrite(WINDOW_IN2,open?LOW:HIGH);
  windowStatus=open;
  isWindowMoving=true;
  windowTimer=millis();
}
void stopWindow(){
  digitalWrite(WINDOW_EN,LOW);
  digitalWrite(WINDOW_IN1,LOW);
  digitalWrite(WINDOW_IN2,LOW);
  isWindowMoving=false;
  windowStatus=false;
}

void fanOn(){
  digitalWrite(FAN_EN,HIGH);
  digitalWrite(FAN_IN1,HIGH);
  digitalWrite(FAN_IN2,LOW); // chỉ chiều dương
  fanStatus=true;
}
void fanOff(){
  digitalWrite(FAN_EN,LOW);
  digitalWrite(FAN_IN1,LOW);
  digitalWrite(FAN_IN2,LOW); // dừng hẳn
  fanStatus=false;
}

// ================= LED & BUZZER =================
void buzzerOn(){digitalWrite(BUZZER_PIN,HIGH); buzzerStatus=true;}
void buzzerOff(){digitalWrite(BUZZER_PIN,LOW); buzzerStatus=false;}
void setLedSafe(){digitalWrite(LED_GREEN,HIGH); digitalWrite(LED_YELLOW,LOW); digitalWrite(LED_RED,LOW);}
void setLedWarning(){digitalWrite(LED_GREEN,LOW); digitalWrite(LED_YELLOW,HIGH); digitalWrite(LED_RED,LOW);}
void setLedDanger(){digitalWrite(LED_GREEN,LOW); digitalWrite(LED_YELLOW,LOW); digitalWrite(LED_RED,HIGH);}
void stopAll(){stopDoor();stopWindow();fanOff();buzzerOff();}

// ================= KEYPAD =================
void handleKeypad(){
  char key=keypad.getKey();
  if(!key) return;
  if(key=='*'){ inputPassword=""; tempMsg="DA XOA"; msgTimer=millis();}
  else if(key=='#'){
    if(inputPassword==adminPassword){ startDoor(true); tempMsg="MO CUA"; msgTimer=millis();}
    else{tempMsg="SAI MK!"; inputPassword=""; msgTimer=millis();}
  } else if(inputPassword.length()<4) inputPassword+=key;
}

// ================= SERIAL =================
void handleCommand(String cmd){
  cmd.trim(); 
  cmd.toUpperCase();

  if(cmd=="MODE_AUTO"){
    autoMode = true;
  }
  else if(cmd=="MODE_MANUAL"){
    autoMode = false;
    // Khi chuyển sang thủ công: tắt hết để tránh Arduino autoLogic còn chạy sót
    stopAll();
  }
  else if(cmd=="FAN_ON") fanOn();
  else if(cmd=="FAN_OFF") fanOff();
  else if(cmd=="DOOR_OPEN") startDoor(true);
  else if(cmd=="DOOR_CLOSE") startDoor(false);
  else if(cmd=="WINDOW_OPEN") startWindow(true);
  else if(cmd=="WINDOW_CLOSE") startWindow(false);
  else if(cmd=="BUZZER_ON") buzzerOn();
  else if(cmd=="BUZZER_OFF") buzzerOff();
}

void readSerial(){
  if(Serial.available()){handleCommand(Serial.readStringUntil('\n')); return;}
  if(Serial1.available()){handleCommand(Serial1.readStringUntil('\n')); return;}
}

// ================= NÚT DỪNG RIÊNG =================
void checkStopButtons(){
  if(digitalRead(BTN_DOOR)==LOW) stopDoor();
  if(digitalRead(BTN_FAN)==LOW) fanOff();
  if(digitalRead(BTN_WINDOW)==LOW) stopWindow();
}

// ================= CẬP NHẬT MOTOR =================
void updateMotors(){
  if(isDoorMoving && millis()-doorTimer>=MOTOR_TIME) stopDoor();
  if(isWindowMoving && millis()-windowTimer>=MOTOR_TIME) stopWindow();
}
