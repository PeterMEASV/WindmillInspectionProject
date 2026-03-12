# Mindst2Commits – Windmill Inspection App

A simple exam application assigned by **SEA** for the **4th Semester Fullstack Exam Project**.

This project was created in collaboration with:
* David Alfred Fyhn Jonas
* Victor Soelberg Møller Jensen

---

## Data Source

The application receives telemetry data from a **simulated MQTT broker** hosted at:

https://sea-fullstack.web.app

**Farm ID:** `Mindst2Commits`

---

## Architecture

The system consists of a **backend service**, a **database**, and a **frontend client**.

### Backend

* Built with **C# .NET 10**
* Receives telemetry data from the MQTT broker
* Saves incoming data to a **PostgreSQL database**
* Streams data to the frontend in real time

### Database

* **PostgreSQL**
* Hosted by **Neon**

### Frontend

* Built with **React + TypeScript**
* Data visualized using **graph components from MUI**
* Displays telemetry and alert information from the backend

---

## Deployment

Both applications are deployed using **Fly.io**.

| Service | URL                                  |
| ------- | ------------------------------------ |
| Client  | https://m2c-windmills-client.fly.dev |
| Server  | https://m2c-windmills-server.fly.dev |

---

## Client Test Credentials

Use the following credentials to log in to the client application:

```
Email: Operator@gmail.com
Password: Operator
```

---

## Alerts

Alerts in the application **are not generated automatically**.

They can be **manually triggered** using the **Swagger UI (NSwag)** available at:

https://m2c-windmills-server.fly.dev/swagger

---

## Tech Stack

* **Backend:** C# .NET 10
* **Frontend:** React + TypeScript
* **Database:** PostgreSQL (Neon)
* **Deployment:** Fly.io
* **API Documentation:** NSwag / Swagger
* **UI Components:** MUI
