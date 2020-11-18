# gaze-detection
Gaze detection in Unity with GUI
Al fine di ottenere una buona gaze detection sono necesserie le seguenti condizioni al contorno:
-Buona luminosità ambientale
-webcam hd
-Buon contrasto
-Presenza di occhiali
Inoltre la buona riuscita dell'interazione con l'interfaccia può essere influenzata dalla forma dell'occhio. Nel caso specifico, al fine di garantire la miglior prestazione possibile, è stato introdotto un fattore correttivo nel calcolo della coordinata y (ciò è dovuto ai limiti della rete nel riconoscimento dell'occhio sotto determinate condizioni).
