# The Specter's Path 👻 | El JUEGO | LEER SECCION DE COMO COMPILAR

![Unity](https://img.shields.io/badge/Unity-6000.0.4.3f1-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-%23178600.svg?style=for-the-badge&logo=csharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-En_Desarrollo-orange?style=for-the-badge)

**The Specter's Path** es un videojuego de aventura y plataformas en perspectiva **2.5D**, desarrollado como proyecto individual para el curso de Programación de Videojuegos 2026 en la **Universidad Nacional Mayor de San Marcos (UNMSM)**.

## 📝 Descripción
El juego combina una estética **HD-2D** (personajes en 2D en entornos 3D) con mecánicas modernas de control. El jugador controla a un espectro que debe navegar por terrenos irregulares, utilizando un sistema de movimiento basado en físicas precisas y una cámara cinematográfica inteligente proporcionada por el packquete cinemachine

##  Características Técnicas
Este proyecto destaca por la implementación de estándares modernos de la industria en Unity 6:

* **Input System Package:** Manejo de entradas basado en acciones (Action-based), permitiendo un control fluido y escalable.
* **Cinemachine:** Sistema de cámara inteligente con algoritmos de *Damping* para un seguimiento suave del personaje.
* **Universal Render Pipeline (URP):** Uso de un flujo de renderizado optimizado para efectos visuales modernos y rendimiento eficiente.
## 📁 Estructura del Proyecto

El contenido está organizado siguiendo una jerarquía profesional para facilitar la escalabilidad:

* `Assets/PROJECT/CODE`: Scripts de C# .
* `Assets/PROJECT/ART`: Sprites, animaciones, materiales y texturas.
* `Assets/PROJECT/WORLD`: Datos de terreno, prefabs de entorno y niveles.

##  Autor , Yo p xd

* **Mark Christian Quispe Gonzales** - *Desarrollo Integral* - [Mark0poll0](https://github.com/Mark0poll0)
* **Institución:** Universidad Nacional Mayor de San Marcos (UNMSM)
* **Carrera:** Computación Científica - Curso Electivo Creacion de Videojuegos y Desarrollo Movil

## 🛠️ Instalación y Uso ===>  [REQUIREMENTS.md](./requirements.md)
1. Asegúrate de tener instalada la versión exacta de **Unity 6 (6000.0.4.3f1)**.
2. Asegurar de tener instalado Visual Studio 2022 
    * **Desarrollo de escritorio de .NET:** (Necesario para el soporte de C# y scripts del jugador).
    * **Desarrollo de escritorio de con C + + :** (Necesario para scripts del Unity).
    * **Desarrollo de juego con Unity:** (Proporciona las herramientas de integración con el motor).
    * **Desarrollo de juegos con C++ :** (Proporciona manipulacion del scripts de Unity).
2. Clona el repositorio:
   ```bash
   git clone https://github.com/Mark0poll0/Proyecto_videojuegos_2026_individual.git


# COMO COMPILAR  (Importante)

Al clonar el repositorio por primera vez, es normal que Unity abra una escena vacía llamada **"Untitled"**. Para ver el proyecto correctamente, sigue estos pasos:

1. **Abrir la escena DEMO :** En la carpeta *Project*, navega a la siguiente ruta:
   `Assets` > `PROJECT` > `SCENES`
2. **Para compilar :** Haz doble clic en el archivo llamado **DEMO** (icono de Unity) . listo la escena para compilar arriba en el centro en el simbolo de play 
3. **Implementacion de PRESENTACION 1 :** Por Ahora solo hay movimiento WASD


> Si al darle al botón de **Play** no sucede nada, asegúrate de que la escena **DEMO** esté abierta en el hierarchy con el doble click , o solo arrastre la escena al hierarchy ,osea el espacio de la izquierda .

##  Estado del Desarrollo (MVP)
Actualmente, el proyecto se encuentra en fase de prototipo funcional (Mínimo Producto Viable):
- [x] **Sistema de Movimiento:** Implementación de WASD (Input System).
- [x] **Entorno:** Prototipo de terreno irregular para pruebas de colisión.
- [x] **Cámara:** Seguimiento básico con Cinemachine.

## 📅 Bitácora de Avances
- **08/05/2026**: Integración de Cinemachine y configuración del Input System para WASD.
- **09/05/2026**: Limpieza de estructura de carpetas y configuración de Git estable.