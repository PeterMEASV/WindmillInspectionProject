import {TelemetryClient} from "./generated-ts-client.ts";

const isProduction = import.meta.env.PROD;
const prod = "https://m2c-windmills-server.fly.dev";
const dev = "http://localhost:5046"

export const finalUrl = isProduction ? prod : dev;

export const telemetryClient = new TelemetryClient(finalUrl);