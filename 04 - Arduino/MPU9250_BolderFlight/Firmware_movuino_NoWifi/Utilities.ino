void blinkNtimes(int N, int delayVar)
{
  int i = 0;
  static uint32_t prev_ms = millis();

  //USed when the record start
  while (i<=N)
  {
      if (millis() > prev_ms + delayVAr)
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
