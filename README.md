# session-time
SESSIONTIME - RECORD WINDOWS SESSION AND LOCKED/UNLOCKED EVENTS AND PARSE AND DISPLAY COLLECTED DATA

Ivan Golović

This application consists of Windows service that records Windows session events and WPF application that parses resulting data to display session durations and locked/unlocked periods within sessions for selected user.

INTRODUCTION

This article describes one possible approach to recording session events on Windows operating system. It also parses event data to display session durations and locked/unlocked periods. Following session events are recorded: logon, logoff, lock, unlock.
Source code might be useful to developers who need working source code to help them with their own Windows session time solution. Windows service and application might be useful if you want to monitor how much time you spend on computer or at which time you logged in and started working.
Application consists of following parts:
-	SessionTimeMonitor - Windows service that records events
-	SessionTimeViewer - Windows application that displays parsed session durations and locked/unlocked periods within sessions
-	SessionTimeSetupWix - Installer/uninstaller for Windows service and application

SessionTimeMonitor

Main part is a Windows service called SessionTimeMonitor - it handles the OnSessionChange event which is triggered each time when a session change occurs. When event is triggered, record is added by the service to the XML file on the disk. Record contains information about the event, such as datetime, type of event, etc.

SessionTimeViewer

SessionTimeViewer application opens and parses XML file that contains data records written by the SessionTimeMonitor. It displays list of sessions with their start and end datetime along with the locked/unlocked periods within a single session. SessionTimeViewer is implemented as a WPF application.

SessionTimeCommon

Contains shared functionality used from SessionTimeMonitor and SessionTimeViewer.

BACKGROUND

I needed this application to determine how much time I spend on the computer and I wanted to have a customizable application. An alternative approach was to write an application that parses Windows event logs but I concluded that parsing Windows event logs and pairing logon/logoff and lock/unlock events was quite cumbersome.

DEVELOPMENT TOOLS

Initial version of Windows service and WPF application were developed using .NET Framework 4.0 and Visual Studio 2010. Current solution builds in Visual Studio 2017 with WiX support and .NET Framework 4.0.

HOW IT WORKS

The following illustration shows how this approach works:
SessionTimeMonitor (Windows service)
-> writes to - > XML data file -> is read and parsed by - >
SessionTimeViewer (Windows application)

HANDLING THE ONSESSIONCHANGE EVENT

The main part of the solution is a Windows service that records session events to the XML data file, this is done through overriding the OnSessionChange event. Recorded data is kept in form of XML file stored on disk.

XML DATA RECORD

Each recorded event is represented by SessionTrackingParams XML element. Such element is added to XML file when the service starts (OnStart event) for each existing session or when session changes (OnSessionChange event). Such events are recorded by SessionTimeMonitor service and added to XML file.

DISPLAYING SESSION DURATION AND LOCKED/UNLOCKED PERIODS

Parsing of XML file is performed by SessionTimeViewer which is a Windows WPF application that shows following information:
-	sessions - as periods between logon and logoff event within same service run marked by ServiceRunGuid
-	unlocked periods - as periods within same session between: logon and logoff, logon and lock, unlock and logoff, unlock and lock
-	locked periods - as periods within same session between: lock and unlock, lock and logoff
This way it is possible to see when and for how long user's session lasted and when did locked and unlocked periods happen.

POINTS OF INTEREST

-	On some systems it might be necessary that SessionTime is installed by user with Administrator privilege, it might also be necessary that SessionTimeViewer is ran by user with Administrator privilege
-	SessionTimeMonitor utilizes the Cassia library which is "a .NET library for accessing the native Windows Terminal Services API (now the Remote Desktop Services API)" (https://code.google.com/p/cassia/, 2016-01-06), it uses its GetSessions method to obtain session data
-	When computer starts up it is possible that SessionTimeMonitor: starts before user logon - in such case user logon will be recorded through OnSessionChange event, starts after user logon - in such case all existing sessions are recorded through service's OnStart event
-	Service and application have been tested on Windows 7 (32 bit), Windows 8.1 (64 bit), Windows 10 (64-bit)
-	Application requires currently used .NET Framework runtime to be installed on computer

HISTORY

-	1.0.0.0 - 2016-06-01 - Initial version
-	1.0.0.1 - 2016-06-01 - Bugfix for version 1.0.0.0, previous version wasn't logging logoff event correctly due to last-minute untested change
-	1.0.0.2 – various bugfixes, user selection, Wix setup introduced
-	1.0.0.3 – various improvements: added "about" window with version/author/repository/documentation info, replaced .docx with .txt for documentation file, improved quality of documentation, moved error log to folder Environment.SpecialFolder.LocalApplicationData, added application icon, global variable moved to GlobalSettings class, removed deprecated SessionTimeSetup project
-	1.0.0.4 – renaming in UI to improve correctness and consistency of terms used in application and documentation, showing of exception in message to the user, displaying of versions of components in "about" window
-	1.0.0.5 – added help link that links to documentation file

Screenshot (viewer application):
![screenshot](./screenshot.png?raw=true)

