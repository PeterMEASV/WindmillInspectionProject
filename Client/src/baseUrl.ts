import {AlertsClient, AuthClient, M2CMqttClient, TelemetryClient} from "./generated-ts-client.ts";
import {forceLogout, TOKEN_KEY, tokenStorage} from "./Token.tsx";

const isProduction = import.meta.env.PROD;
const prod = "https://m2c-windmills-server.fly.dev";
const dev = "http://localhost:5046"

const customFetch = async (url: RequestInfo, init?: RequestInit) => {
    const token = tokenStorage.getItem(TOKEN_KEY, null);

    const redirectToLogin = () => {
        if (window.location.pathname !== "/login") {
            const next = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.replace(`/login?next=${next}`);
        }
    };
    
    if (token) {
        // Copy of existing init or new object, with copy of existing headers or
        // new object including Bearer token.
        init = {
            ...(init ?? {}),
            headers: {
                ...(init?.headers ?? {}),
                Authorization: `Bearer ${token}`,
            },
        };
    }
    const response = await fetch(url, init);

    if (response.status === 401 || response.status === 403) {
        forceLogout();
        redirectToLogin();
    }
    return response;
   
};
export const finalUrl = isProduction ? prod : dev;

export const telemetryClient = new TelemetryClient(finalUrl, { fetch: customFetch });
export const alertClient = new AlertsClient(finalUrl, { fetch: customFetch });

export const MqttClient = new M2CMqttClient(finalUrl, { fetch: customFetch });

export const authClient = new AuthClient(finalUrl, { fetch: customFetch });
