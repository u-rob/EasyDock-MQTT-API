# EasyDock-MQTT-API

Die API basiert auf JSON-Nachrichten, die über einen MQTT-Broker versendet
werden.

Folgende topics werden verwendet:

- easydock-api/sn/{dock-sn}/request – Dieses Topic wird für API-Anfragen verwendet
- easydock-api/sn/{dock-sn}/reply   – Dieses Topic wird für API-Antworten verwendet

Jeder Aufruf muss die folgende Spezifikation erfüllen:

- message_id (string) – Für jede Nachricht einzigartige ID
- timestamp (long) – Aktuelle Zeit in Millisekunden
- method (string) – Die auszuführende Aktion
  - Verfügbare Aktionen:
    - start_mission
    - takeoff_to_point
- dock_sn (string) – SN des Docks, mit welchem die Aktion durchgeführt werden soll.
- data (JSON-Object) – Die Parameter für die Aktion

Jede Antwort erfüllt die folgende Spezifikation:
- message_id (string) – Einzigartige ID der Nachricht
- timestamp (number) – Zeitstempel, an welchem die Antwort verschickt wurde
- reply_to (string) – message_id der Aufrufnachricht
- status_code (number) – Numerischer Wert, welcher das Ergebnis darstellt
  - Mögliche Werte    
    - 0 – Error: (Beispiel: Manuelle Steuerung ist noch aktiv)
    - 1 – Aufruf erfolgreich bearbeitet
- status_code_description (string) – Beschreibung des Status in Textform

## Aufrufe
### start_mission
Request:

````json
{
    "message_id": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "timestamp": 1715778874224,
    "dock_sn": "7ABDE9081232190F",
    "method": "start_mission",
    "data": {
      "mission_name": "Inspektionsflug"
      "mission_by_easydock": true,
      "signal_type": (int)SignalType.ALARM,
      "checklist_confirmed": false,
      "user_id": "123"
    }
}
````

Reply (Erfolg):

````json
{
    "message_id": "f09a1e70-5c56-45c0-ad77-879c6183cad1",
    "timestamp": 1715779016020,
    "replying_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 1,
    "status_code_description": "success"
}
````

Reply (Fehler):

````json
{
    "message_id": "a7fd55d4-9696-4d3e-bcd6-2a69029517c4",
    "timestamp": 1715779149528,
    "reply_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 0,
    "status_code_description": "Manuelle Steuerung ist noch aktiv"
}
````

### takeoff_to_point

Request:

````json
{
    "message_id": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "timestamp": 1715778874224,
    "dock_sn": "7ABDE9081232190F",
    "method": "takeoff_to_point",
    "data": {
      "lat": 52.084373,
      "lng": 8.5098026,
      "height": 50,
      "signal_type": (int)SignalType.ALARM,
      "checklist_confirmed": false,
      "user_id": "123"
    }
}
````

Reply (Erfolg):

````json
{
    "message_id": "f09a1e70-5c56-45c0-ad77-879c6183cad1",
    "timestamp": 1715779016020,
    "reply_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 1,
    "status_code_description": "success"
}
````

Reply (Fehler):

````json
{
    "message_id": "a7fd55d4-9696-4d3e-bcd6-2a69029517c4",
    "timestamp": 1715779149528,
    "reply_to": "c2b865b6-478c-4b9f-80fe-1bcd98b20b91",
    "status_code": 0,
    "status_code_description": "Manuelle Steuerung ist noch aktiv"
}
````
