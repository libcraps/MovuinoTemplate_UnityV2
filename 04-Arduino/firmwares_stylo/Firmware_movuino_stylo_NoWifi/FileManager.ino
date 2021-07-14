/*
 * Functions that allows us to manage files in the firmware
 */



void createFile(String filepath)
{
  /*
   * Create the file for the movuino with the filepath "filepath"
   */
  file = SPIFFS.open(filepath, "w");
 
  if (!file) {
    Serial.println("Error opening file for writing");
    return;
  }
  initialiseFileMovuinoData(file, sep);
}

void addNewRecord(String filepath)
{
    file = SPIFFS.open(filePath, "a");     
    if (!file) 
    {
      Serial.println("Error opening file for writing");
      return;
    }
    file.println("XXX_newRecord");
    initialiseFileMovuinoData(file, sep);
}

void readFile(String filepath)
{
  
  File file = SPIFFS.open(filepath, "r");
  
  if (!file) {
    Serial.println("Error opening file for reading");
    return;
  }
  Serial.println("XXX_beginning");
  while(file.available()){
    Serial.write(file.read());
  }
  file.close();
  Serial.println("XXX_end");
}

void writeData(String filePath)
{
  
  file = SPIFFS.open(filePath, "a");
  
  if (!file) 
  {
    Serial.println();
    Serial.println("Error opening file for writing");
    return;
  }
  
  digitalWrite(pinLedBat, HIGH);
  writeInFileMovuinoData(file, sep);
  file.close();
}

void listingDir(String dirPath)
{
  Serial.println("Listing dir :");
  Dir dir = SPIFFS.openDir(dirPath);
  while (dir.next()) 
  {
    Serial.print(dir.fileName());
    File f = dir.openFile("r");
    Serial.print(" ");
    Serial.println(f.size());
    f.close();
  }
  Serial.println("End of listing");
}

void formatingSPIFFS(){
  bool formatted = SPIFFS.format();
  if(formatted)
  {
    Serial.println("\n\nSuccess formatting");
  }
  else
  {
    Serial.println("\n\nError formatting");
  }
}

void getInfoAboutSpiff(){
      // Get all information about SPIFFS
    FSInfo fsInfo;
    SPIFFS.info(fsInfo);
    
    Serial.println("File system info");
    
    // Taille de la zone de fichier
    Serial.print("Total space:      ");
    Serial.print(fsInfo.totalBytes);
    Serial.println("byte");
    
    // Espace total utilise
    Serial.print("Total space used: ");
    Serial.print(fsInfo.usedBytes);
    Serial.println("byte");
 
    // Taille d un bloc et page
    Serial.print("Block size:       ");
    Serial.print(fsInfo.blockSize);
    Serial.println("byte");
 
    Serial.print("Page size:        ");
    Serial.print(fsInfo.totalBytes);
    Serial.println("byte");
 
    Serial.print("Max open files:   ");
    Serial.println(fsInfo.maxOpenFiles);
 
    // Taille max. d un chemin
    Serial.print("Max path lenght:  ");
    Serial.println(fsInfo.maxPathLength);
 
    Serial.println();
}
