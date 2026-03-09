import {atom} from "jotai";
import type {RouteObject} from "react-router";
import Home from "./Home.tsx";
import Login from "./Login.tsx";
import Dashboard from "./Dashboard.tsx";

export const routesAtom = atom<RouteObject[]>([
    {
        path: "/",
        element: <Home />
    },
    {
        path: "login",
        element: <Login />
    },
    {
        path: "Dashboard",
        element: <Dashboard />
    }
])