void blink3Times()
{
  //USed when the record start
  digitalWrite(pinLedESP, LOW);
  delay(250);
  digitalWrite(pinLedESP, HIGH);
  delay(250);
  digitalWrite(pinLedESP, LOW);
  delay(250);
  digitalWrite(pinLedESP, HIGH);
  delay(250);
  digitalWrite(pinLedESP, LOW);
  delay(250);
  digitalWrite(pinLedESP, HIGH);
  delay(250);
  digitalWrite(pinLedESP, LOW);
}
void blinkLongTimes()
{
  //Used when the record stop
  digitalWrite(pinLedESP, LOW);
  delay(500);
  digitalWrite(pinLedESP, HIGH);
  delay(1000);
  digitalWrite(pinLedESP, LOW);
  delay(500);
  digitalWrite(pinLedESP, HIGH);
  delay(1000);
  digitalWrite(pinLedESP, LOW);
  delay(500);
  digitalWrite(pinLedESP, HIGH);
  delay(1000);
  digitalWrite(pinLedESP, LOW);
}

void blinkNtimes(int N, int delayVar)
{
  int i = 0;
  static uint32_t prev_ms = millis();

  //USed when the record start
  while (i<=N)
  {
      if (millis() > prev_ms + delayVar)
      {
          digitalWrite(pinLedESP, LOW);
          prev_ms = millis();
          i++;
      }
      else 
      {
          digitalWrite(pinLedESP, HIGH);
      }
  }
}
