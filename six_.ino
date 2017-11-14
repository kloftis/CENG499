#include <FreeSixIMU.h>
#include <FIMU_ADXL345.h>
#include <FIMU_ITG3200.h>

#include <Wire.h>

float angles[3]; // yaw pitch roll
float ypr[3]; // yaw, pitch, roll

// Set the FreeSixIMU object
FreeSixIMU sixDOF = FreeSixIMU();

// Push button pin assignment
const int buttonPin0 = 6;
const int buttonPin1 = 2;     
const int buttonPin2 = 3;
// Scroll wheel pin assignment
const int scrollPin0 = 4;     
const int scrollPin1 = 5;

int buttonState0 = 0;
int buttonState1 = 0;         // variable for reading the pushbutton status
int buttonState2 = 0;         // variable for reading the pushbutton status
int scrollState0 = 0;
int scrollState1 = 0;

int buttonStatePrev0 = 0;
int buttonStatePrev1 = 0;         // variable for reading the pushbutton status
int buttonStatePrev2 = 0;         // variable for reading the pushbutton status
int scrollStatePrev0 = 0;
int scrollStatePrev1 = 0;


void setup() { 
  Serial.begin(9600);
  Wire.begin();
  
  //pinMode(ledPin, OUTPUT);
  pinMode(buttonPin0, INPUT);  
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(scrollPin0, INPUT);
  pinMode(scrollPin1, INPUT);
  
  sixDOF.init(); // begin the IMU
  delay(5);
}

void loop() { 
  
  buttonState0 = digitalRead(buttonPin0);
  buttonState1 = digitalRead(buttonPin1);
  buttonState2 = digitalRead(buttonPin2);
  scrollState0 = digitalRead(scrollPin0);
  scrollState1 = digitalRead(scrollPin1);
  
  //  sixDOF.getEuler(angles);
  //  sixDOF.getAngles(angles);
  if(buttonState0 == 1)
      sixDOF.getYawPitchRoll(ypr); // Keep this outside the below if statement so that the values are measured even when the button isn't pressed
  
  
 scrollState0 = digitalRead(scrollPin0);
 scrollState1 = digitalRead(scrollPin1);
  
  //print all data in one line
  //[Activate Gyro][Button1][Button2][Scroll1]Scroll2][Yaw]|[Roll]|[Pitch]
  if((buttonState0 == 1) || (buttonState1 != buttonStatePrev1) || (buttonState2 != buttonStatePrev2) || (scrollState1 != scrollStatePrev1) || (scrollState0 != scrollStatePrev0))
  {
      Serial.print(buttonState0);
      Serial.print("|");
      Serial.print(buttonState1);
      Serial.print("|");
      Serial.print(buttonState2);
      Serial.print("|");
      
      Serial.print(scrollState0);
      Serial.print(scrollState1);
      Serial.print("|");
      
      Serial.print(ypr[0]);
      Serial.print("|");  
      Serial.print(ypr[1]);
      Serial.print("|");
      Serial.print(ypr[2]);

            
      Serial.print("\n");

      buttonStatePrev0 = buttonState0;
      buttonStatePrev1 = buttonState1;         // variable for reading the pushbutton status
      buttonStatePrev2 = buttonState2;         // variable for reading the pushbutton status
      scrollStatePrev0 = scrollState0;
      scrollStatePrev1 = scrollState1;

      delay(1);      
      
  }
 
}

