# Software Architektur

>LiftDataManager folgt dem **MVVM** Datenbank(**Model - View - ViewModel**), dadurch wird eine lose Kopplung zwischen der der Benutzerschnittstelle (UI) und des Software-Backends erreicht.  
>Durch das MVVM Muster ist gewährleistet das die Anwendung ein Höchstmaß an Wartbarkeit, Wiederverwendbarkeit, Skalierbarkeit und Testbarkeit erhält.

![image](/LiftDataManager/Docs/HelpImages/image184.png)  

---  

>Die Benutzerschnittstelle (**View**) wurde mit **Windows-App SDK** und **WinUI 3** realisiert, welches aktuell das modernste UI Framework für Windows Desktop Anwendungen darstellt.  

---  

>Durch WinUI 3 fügt sich LiftDataManager grafisch nahtlos in Windows11 ein.  

---  

>Die Geschäftslogik der Anwendung (**Model**) beruht auf der **.NET Plattform** (aktuell .NET 8) und wurde in **Programmiersprache C#** geschrieben.  

---  

>Die Abhängigkeiten werden mit **Dependency Injection** übergeben, hierdurch ist es möglich Single-Responsibility-Prinzip als Entwurfsrichtlinie einzusetzen.  

---  

>Die Implementierung der Datenbank erfolgte mit **Entity Framework Core** im Code First Ansatz.

[//]: # (Tags: # Software Architektur | MVVM | Model - View - ViewModel | Windows-App SDK | WinUI 3 | .NET Plattform | Entwurfsrichtlinie |Dependency Injection | Entity Framework Core)  
