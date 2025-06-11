# Registry Reader Plugin for InfoPanel

A plugin for InfoPanel to display up to five strings and numbers from your registry.

## Installation and Setup
Follow these steps to get the ping plugin working with InfoPanel:

1. **Download the plugin**:
   - Download the latest release \*.zip file (`InfoPanel.RegistryReader-vX.X.X.zip`) from the [GitHub Releases page](https://github.com/markuskonojacki/InfoPanel.RegistryReader/releases).

2. **Import the Plugin into InfoPanel**:
   - Open the InfoPanel app.
   - Navigate to the **Plugins** page.
   - Click **Import Plugin Archive**, then select the downloaded ZIP file.
   - InfoPanel will extract and install the plugin.

3. **Configure the Plugin**:
   - On the Plugins page, click **Open Plugins Folder** to locate the plugin files.
   - Close InfoPanel.
   - Open `InfoPanel.RegistryReader.dll.ini` in a text editor (e.g., Notepad).
   - Add the desired number of IPs or server addresses as a comma seperated list.
   - Save and close the file.

## Configuration example

- **`InfoPanel.RegistryReader.dll.ini`**:
  ```ini
  [RegistryReader Plugin]
  RefreshTimer = 10
  String01Path = HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProductName
  String02Path = HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\DisplayVersion
  String03Path = 
  String04Path = 
  String05Path = 
  DWord01Path = 
  DWord02Path = 
  DWord03Path = 
  DWord04Path = 
  DWord05Path = 