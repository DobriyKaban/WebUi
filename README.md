# WebUI Project
---
# ДИСКЛЕЙМЕР!/DISCLAIMER!

EN: This code is a Harmony patch for space station 14 for marsey loader, the cool thing about this repository is that I integrated WebView2 into the patch so that you can write menus in html, js, ts, React, and so on. The functionality is simply CerberusWareV3

RU: Данный код это Harmony патч для space station 14 для marsey loaderприкол данного репозитория в том что я интегрировал WebView2 в патч, что бы можно было писать меню на html, js, ts, React и тд. Функционал это просто CerberusWareV3

---
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Vite](https://img.shields.io/badge/vite-%23646CFF.svg?style=for-the-badge&logo=vite&logoColor=white)
![NPM](https://img.shields.io/badge/NPM-%23CB3837.svg?style=for-the-badge&logo=npm&logoColor=white)

---

### Select Language / Выберите язык:
## [🇺🇸 English](#english-compilation-guide) &nbsp;&nbsp;|&nbsp;&nbsp; [🇷🇺 Русский](#руководство-по-компиляции)

---

<a name="english-compilation-guide"></a>
# 🇺🇸 English: Compilation Guide

## 1. Web Interface (Menu) Compilation

Before building the main project, you must compile the frontend interface.

### Steps:

1.  Navigate to the project source directory:
    `WebUi/WebUi/Resources/Web/decomp`

2.  Install dependencies:
    ```bash
    npm install
    ```

3.  **Important:** Install the `vite-plugin-singlefile` plugin:
    ```bash
    npm install vite-plugin-singlefile --save-dev
    ```

4.  Build the project:
    ```bash
    npm run build
    ```

> [!WARNING]
> **DO NOT** replace or modify the `vite.config` file under any circumstances!

5.  **Finalize Web Assets:**
    *   Locate the `index.html` file in the generated `dist` folder.
    *   Move it to: `WebUi/WebUi/Resources/Web/`
    *   *Expected result:* The file path should be `WebUi/WebUi/Resources/Web/index.html`.

---

## 2. Project Compilation & Installation

### Build

Compile the main project using your IDE (Visual Studio, Rider) or CLI.
*   **Target Framework:** .NET 9.0

### Installation (Deploy to Marsey)

After a successful build, the output files will be located in:
`WebUi/WebUi/bin/Debug/net9.0/`

**Follow these steps to install:**

1.  **Main Files:**
    Copy all files from `WebUi/WebUi/bin/Debug/net9.0/` to **Mods** folder.

2.  **Assembly Resources:**
    Copy all files from `WebUi/WebUi/bin/Debug/net9.0/Resources/Assembly/` to **Mods** folder as well.

> [!NOTE]
> It is recommended to move all files if you are unsure which specific ones are required.

---
---

<a name="руководство-по-компиляции"></a>
# 🇷🇺 Русский: Руководство по компиляции

## 1. Компиляция Веб-интерфейса (Меню)

Перед сборкой основного проекта необходимо скомпилировать фронтенд часть.

### Шаги:

1.  Перейдите в директорию с исходным кодом интерфейса:
    `WebUi/WebUi/Resources/Web/decomp`

2.  Установите зависимости:
    ```bash
    npm install
    ```

3.  **Важно:** Установите плагин `vite-plugin-singlefile`:
    ```bash
    npm install vite-plugin-singlefile --save-dev
    ```

4.  Соберите проект:
    ```bash
    npm run build
    ```

> [!WARNING]
> **НИ В КОЕМ СЛУЧАЕ** не заменяйте и не удаляйте файл `vite.config`!

5.  **Перенос файлов:**
    *   Найдите файл `index.html` в появившейся папке `dist`.
    *   Переместите его в папку: `WebUi/WebUi/Resources/Web/`
    *   *Итог:* Файл должен находиться по пути `WebUi/WebUi/Resources/Web/index.html`.

---

## 2. Компиляция проекта и Установка

### Сборка

Скомпилируйте основной проект, используя вашу IDE (Visual Studio, Rider) или консоль.
*   **Платформа:** .NET 9.0

### Установка (Перенос в Marsey)

После успешной компиляции файлы сборки будут находиться в:
`WebUi/WebUi/bin/Debug/net9.0/`

**Инструкция по установке:**

1.  **Основные файлы:**
    Скопируйте все файлы из папки `WebUi/WebUi/bin/Debug/net9.0/` в папку с патчами **Mods**.

2.  **Ресурсы сборки:**
    Скопируйте все файлы из папки `WebUi/WebUi/bin/Debug/net9.0/Resources/Assembly/` также в папку с патчами **Mods**.

> [!NOTE]
> Желательно перенести абсолютно все файлы из указанных папок, чтобы избежать ошибок отсутствия зависимостей.
