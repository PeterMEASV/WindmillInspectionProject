import {atom} from "jotai";
import type {RouteObject} from "react-router";
import Home from "./Home.tsx";
import Dashboard from "./Dashboard.tsx";
import {LoginPage} from "./LoginPage.tsx";

export const connectionIdAtom = atom<string | null>(null);
export const routesAtom = atom<RouteObject[]>([
    {
        path: "/",
        element: <Home />
    },
    {
        path: "Dashboard",
        element: <Dashboard />
    },
    {
        path: "Login",
        element: <LoginPage />
    }
])