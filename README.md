# 🧠 AR Tooltip Interaction App

An interactive AR app built with Unity, designed to recognize horizontal planes and place/manipulate a 3D model with contextual tooltips.

## 📲 How to Use the App

### ✅ Requirements

* **Unity Version**: 6000.1.9f1 or compatible
* **Platform**: iOS / Android (AR-capable device)
* **Required Packages**:

  * AR Foundation
  * ARCore XR Plugin (Android)
  * ARKit XR Plugin (iOS)
  * XR Plugin Management

---

### 🚀 Launching the App

1. Open the app on an AR-supported mobile device.
2. Move your phone around to allow the app to detect horizontal surfaces (planes).
3. Once a plane is detected:

   * **Single Tap** on the plane to **place the 3D model**.
   * **Double Tap** on the placed model to **remove** it.

---

### 🔴 Tooltip Interaction

The loaded 3D model contains red spheres positioned on specific areas.
These act as **interactive hotspots**:

* **Tap on a red sphere** to display **contextual information** via an AR tooltip.
* The tooltip appears in world space and follows the camera's facing direction.

📦 You can download the 3D model used in the app [here](https://www.turbosquid.com/it/3d-models/3d-model-audi-a7-sportback-2018-55-tfsi-2430054).

---

### ✋ Object Manipulation

A floating UI lets you interact with the placed object in real-time:

* Three buttons allow (along the detected AR plane):

  * **Translate**
  * **Rotate**
  * **Scale**


* A toggle in the top-right corner lets you enable **auto-detection mode**:

  * In this mode, you can use **two-finger gestures** to automatically switch between translation, rotation, and scaling based on the gesture type.

---

## 🧪 Project Overview (for Developers)

This Unity project demonstrates:

* AR plane detection and interaction using AR Foundation
* Tooltip display using the [Simple Tooltip](https://assetstore.unity.com/packages/tools/gui/simple-tooltip-system-147860) system
* Object manipulation [UI](https://assetstore.unity.com/packages/2d/gui/violet-themed-ui-235559) with gesture-based alternative
* Dynamic tooltip management on model subcomponents (via red spheres)

### 💡 Suggestions for Future Improvements

* Add support for multiple model placements
* Allow users to upload custom models at runtime
* Implement vertical plane detection and interaction
* Improve UX with animation or sound feedback when tooltips appear
* Save the model’s transformation state between sessions

---

## 🧩 Key Components

* **ARTouchTooltip.cs**
  Detects user touch input and toggles tooltip visibility on objects.

* **SimpleTooltip Integration**
  Handles tooltip visuals and positioning in AR space.

* **ObjectManipulator.cs**
  Manages transformation (move/rotate/scale) of the placed model.

* **ARPlacementManager.cs**
  Handles plane detection and object instantiation/removal.

---

## 🐛 Troubleshooting Tips

**Model not placed?**

* Make sure the environment is well-lit and has visible horizontal surfaces.

**Tooltips not appearing?**

* Ensure red spheres are properly tagged and have colliders.
* Confirm the SimpleTooltip prefab is in the Resources folder.

**Object manipulation not working?**

* Try toggling between manual UI and gesture mode.
* Make sure the AR session is stable and tracking properly.

---

## 📄 License & Credits

* **Simple Tooltip Assets**: [CC0 by Kenney](Assets/Simple%20Tooltip/Assets/Sprites/license.txt)
* **Fonts**: [SIL Open Font License](Assets/Simple%20Tooltip/Assets/Font/OFL.txt)
* **3D Model**: [Standard License](https://blog.turbosquid.com/turbosquid-3d-model-license/)

---

## 🤝 Contributing

This is a prototype project for a technical test. Feel free to fork, suggest features, or open pull requests to extend functionality!

---

---

# 🇮🇹 VERSIONE ITALIANA

# 🧠 App di Interazione AR con Tooltip

Un'app AR interattiva sviluppata con Unity, progettata per riconoscere superfici orizzontali e posizionare/manipolare un modello 3D con tooltip contestuali.

---

## 📲 Come Usare l'App

### ✅ Requisiti

* **Versione di Unity**: 6000.1.9f1 o compatibile
* **Piattaforma**: iOS / Android (dispositivo compatibile con AR)
* **Pacchetti Necessari**:

  * AR Foundation
  * ARCore XR Plugin (Android)
  * ARKit XR Plugin (iOS)
  * XR Plugin Management

---

### 🚀 Avvio dell'App

1. Apri l'app su un dispositivo mobile compatibile con AR.
2. Muovi il telefono nell’ambiente per permettere il rilevamento delle superfici orizzontali.
3. Una volta rilevato un piano:

   * **Tocca una volta** sul piano per **posizionare il modello 3D**.
   * **Tocca due volte** sul modello per **rimuoverlo**.

---

### 🔴 Interazione con Tooltip

Il modello 3D contiene sfere rosse posizionate in aree specifiche.
Queste agiscono come **punti interattivi**:

* **Tocca una sfera rossa** per visualizzare **informazioni contestuali** tramite un tooltip in AR.
* Il tooltip appare nello spazio tridimensionale e segue la direzione della fotocamera.

📦 Puoi scaricare il modello 3D usato nell'app [qui](https://www.turbosquid.com/it/3d-models/3d-model-audi-a7-sportback-2018-55-tfsi-2430054).

---

### ✋ Manipolazione dell'Oggetto

Un'interfaccia fluttuante consente di interagire in tempo reale con l'oggetto posizionato:

* Tre pulsanti permettono di (rispetto al piano individuato):

  * **Traslare**
  * **Ruotare**
  * **Scalare**

* Un interruttore in alto a destra attiva la **modalità di auto-rilevamento**:

  * In questa modalità, puoi usare **gesture a due dita** per passare automaticamente tra traslazione, rotazione e scala in base al tipo di gesto.

---

## 🧪 Panoramica del Progetto (per Sviluppatori)

Questo progetto Unity dimostra:

* Rilevamento di piani e interazione tramite AR Foundation
* Visualizzazione di tooltip usando il sistema [Simple Tooltip](https://assetstore.unity.com/packages/tools/gui/simple-tooltip-system-147860)
* UI per la manipolazione dell’oggetto con alternativa basata su gesture
* Gestione dinamica dei tooltip sui sottocomponenti del modello (tramite sfere rosse)

### 💡 Suggerimenti per Miglioramenti Futuri

* Supportare il posizionamento di più modelli
* Permettere il caricamento di modelli personalizzati a runtime
* Implementare il rilevamento e l’interazione con piani verticali
* Migliorare la UX con animazioni o effetti sonori alla comparsa dei tooltip
* Salvare lo stato di trasformazione del modello tra le sessioni

---

## 🧩 Componenti Chiave

* **ARTouchTooltip.cs**
  Rileva l'input dell'utente e gestisce la visibilità dei tooltip sugli oggetti.

* **Integrazione Simple Tooltip**
  Gestisce la visualizzazione e il posizionamento dei tooltip nello spazio AR.

* **ObjectManipulator.cs**
  Gestisce le trasformazioni (traslazione/rotazione/scalatura) del modello posizionato.

* **ARPlacementManager.cs**
  Gestisce il rilevamento dei piani e l’istanziazione/rimozione degli oggetti.

---

## 🐛 Suggerimenti per la Risoluzione dei Problemi

**Il modello non viene posizionato?**

* Assicurati che l’ambiente sia ben illuminato e presenti superfici orizzontali visibili.

**I tooltip non appaiono?**

* Verifica che le sfere rosse siano correttamente etichettate e abbiano dei collider.
* Assicurati che il prefab SimpleTooltip si trovi nella cartella `Resources`.

**La manipolazione dell’oggetto non funziona?**

* Prova a passare dalla modalità UI manuale alla modalità gesture.
* Verifica che la sessione AR sia stabile e che il tracciamento funzioni correttamente.

---

## 📄 Licenza e Crediti

* **Simple Tooltip Assets**: [CC0 by Kenney](Assets/Simple%20Tooltip/Assets/Sprites/license.txt)
* **Font**: [SIL Open Font License](Assets/Simple%20Tooltip/Assets/Font/OFL.txt)
* **Modello 3D**: [Licenza Standard](https://blog.turbosquid.com/turbosquid-3d-model-license/)

---

## 🤝 Contributi

Questo è un progetto prototipale per un test tecnico. Sentiti libero di fare fork, proporre funzionalità o aprire pull request per estendere le funzionalità!

---
