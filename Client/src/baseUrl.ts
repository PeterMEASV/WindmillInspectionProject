import {AlertsClient, M2CMqttClient, TelemetryClient} from "./generated-ts-client.ts";

const isProduction = import.meta.env.PROD;
const prod = "https://m2c-windmills-server.fly.dev";
const dev = "http://localhost:5046"

export const finalUrl = isProduction ? prod : dev;

export const telemetryClient = new TelemetryClient(finalUrl);
export const alertClient = new AlertsClient(finalUrl);

export const MqttClient = new M2CMqttClient(finalUrl);