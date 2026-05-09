# Requerimientos Técnicos - The Specter's Path

Este documento detalla las dependencias y especificaciones necesarias para ejecutar y desarrollar el proyecto.

## 1. Información del Proyecto
* **Repositorio Oficial:** [https://github.com/Mark0poll0/Proyecto_videojuegos_2026_individual](https://github.com/Mark0poll0/Proyecto_videojuegos_2026_individual)
* **Desarrollador:** Mark Christian Quispe Gonzales

## 2. Entorno de Desarrollo (Software)
Para abrir, compilar y modificar el proyecto, se requiere el siguiente software y cargas de trabajo (Workloads):

* **Motor de Juego:** Unity 6 (Versión exacta: **6000.0.4.3f1**).
* **Editor de Código:**  Visual Studio 2022.
* **Cargas de trabajo necesarias (Visual Studio Installer):**
    * **Desarrollo de escritorio de .NET:** (Necesario para el soporte de C# y scripts del jugador).
    * **Desarrollo de escritorio de con C + + :** (Necesario para scripts del jugador).
    * **Desarrollo de juego con Unity:** (Proporciona las herramientas de integración con el motor).
    * **Desarrollo de juegos con C++ :** (Proporciona manipulacion del scripts).
* **Control de Versiones:** Git.

## 3. Dependencias y Paquetes (Unity Packages)
El proyecto utiliza el gestor de paquetes de Unity deberia instalrse solo . En caso no ser instaldas las dependencias principales instaladas son:

* **Input System:** Utilizado para el mapeo de acciones de movimiento del personaje (sustituye al sistema Legacy).
* **Cinemachine:** Implementado para el seguimiento inteligente de cámara con algoritmos de Damping.
* **2D Sprite:** Para el manejo de los assets visuales y animaciones.

## 4. Requerimientos de Hardware (Estimados)
* **GPU:** Compatible con DirectX 11 o superior (necesario para el sistema de iluminación URP).
* **Memoria RAM:** 8 GB recomendado para el editor de Unity.
* **Almacenamiento:** Espacio suficiente para la regeneración de la carpeta `Library` local.

## 5. Instrucciones de Instalación
1. Clonar el repositorio: 
   `git clone https://github.com/Mark0poll0/Proyecto_videojuegos_2026_individual.git`
2. Abrir **Unity Hub**.
3. Seleccionar "Add" y buscar la carpeta raíz del proyecto.
4. Asegurarse de usar la versión **6000.0.38f1**.