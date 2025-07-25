# System Health API

[![license](https://img.shields.io/badge/license-MIT-blue)](https://opensource.org/license/mit)
[![release](https://img.shields.io/github/v/release/GameMill/WinPE_OS_Installer)](https://github.com/GameMill/SystemHealthAPI/releases)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](https://github.com/GameMill/SystemHealthAPI/pulls)

# 🖥️ System Health API

A lightweight System Health Checker that gathers CPU, GPU, RAM, drive, and network metrics and exposes them via a local HTTP endpoint (with JSONP support).

---

## 📋 Prerequisites

- Windows 10 or later  
- .NET Framework 4.6+ or .NET Core 3.1+  
- Administrator privileges (required for low-level hardware sensors)  

---

## ⚙️ Installation & Setup

1. Clone or download this repository:  
   ```bash
   git clone https://github.com/YourUser/SystemHealthAPI.git
   cd SystemHealthAPI
   ```
2. Build the project in Visual Studio (or via CLI):  
   ```bash
   dotnet build
   ```
3. (Optional) Adjust any settings in `appsettings.json` if provided.

---

## 🚀 How to Use

1. **Launch the application**  
   Run `SystemHealthAPI.exe` (or `dotnet run`); the console will auto-close once the HTTP service is up.

2. **View locally**  
   - Locate the tray icon.  
   - Hover to display the tooltip.  
   - Right-click the tooltip → **Print Info** to pop up the health dashboard.

3. **Query via HTTP/JSONP**  
   Open your browser or HTTP client and navigate to:  
   ```
   http://<YOUR-IP-ADDRESS>:10000/?callback=<YOUR_JSONP_CALLBACK>
   ```

---

## 🖼️ Example JSONP Response

```js
rf21f1({
  "FirstHddRun": { "C:\\": true, "D:\\": true, "E:\\": true, "F:\\": true, "G:\\": true },
  "CPU": {
    "Name": "AMD Ryzen 9 3900X",
    "Cores": [
      { "Name": "CPU Core #1", "Current": 54.64, "Min": 9.38, "Max": 65.46 },
      /* … 11 more cores … */
    ],
    "Total": { "Name": "Total", "Current": 23.32, "Min": 50.63, "Max": 60.13 }
  },
  "Gpu": {
    "Name": "NVIDIA GeForce RTX 3070",
    "Temperature": { "Current": 33.0 },
    "Usage":       { "Current": 23.0 },
    "TotalMemory": 8192,
    "MemoryUsed":  { "Current": 1735.66 }
  },
  "ram": {
    "Load":            { "Current": 46.87 },
    "UsedMemory":      { "Current": 14.96 },
    "AvailableMemory": { "Current": 16.96 }
  },
  "Drives": [
    {
      "Label": "New Volume",
      "Mount": "D:\\",
      "TotalSize":       7452.02,
      "TotalFreeSize":   5187.00,
      "UsedPercentage":  30.39,
      "Temperature":     28.0,
      "Name":            "WDC WD82PURZ-85TEUY0"
    }
    /* … more drives … */
  ],
  "networks": []
});
```

---

## 🔗 Dependency

- [OpenHardwareMonitor](https://github.com/GameMill/openhardwaremonitor) — hardware-sensor library

---
```
