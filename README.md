# UniversalDeviceManager

* [Discord Server](https://discord.gg/eQXvPej8Ms)

## Description

(RU) 

UDM - приложение для автоматизации и упрощения прошивки смартфонов. Главная особенность - собственный скриптовый язык DIL, позволяющий абстрагировать пользователя от кучи перезагрузок и действий в меню. 
Теперь для определенных действий (например, переход на новую прошивку, или установка рут-прав) нужно будет всего лишь скачать UDM, подключить телефон и запустить соответствующий скрипт.

## Installation

* *Light* version weights less that regular, but **might not start** on some PCs (for example, on those that don't have **.Net 8**)
* *Regular* version weights way more than light, but have significantly higher chance to start on **your** PC

  Just unzip applicaition and start `UniversalDeviceManager.exe` / `UDM.WPF.exe`

## DIL Guide

[Documentaion](https://github.com/maksimkh34/UniversalDeviceManager/blob/main/src/DIL_DOCS.md)

## Project structure
```
├── UniversalDeviceManager **(root)**
    ├── cwd
    │   ├── fastboot - fastboot tools
    │   │
    │   ├── config - configuration files
    │   │   ├── init - first start indicator
    │   │   └── settings_storage.conf - settings database
    │   │
    │   ├── python - python scripts
    │   │   └── install.py - first launch script
    │   │
    │   ├── example.dil - example DIL script (do NOT use!)
    │   ├── UniversalDeviceManager.exe - Executable
    │   └── changelog - contains changelog for app to display
    │
    └── src
```
## Donate: 
* **TON:** UQBs1SZLh1YAQXu64dnFk5BlXoDyU9WiZoSb4LZLTcypdsBE *(csoftware)*
* **TRX/TRON:** TVEVKRA2PKb7emH4UM65PCT4L48DFqTZsy *(maksimkh34/gvand)*
* **WMZ:** Z693285176654 *(maksimkh34)*

## Contact
[Telegram](https://t.me/trxshv) (maksimkh34)
