import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {StreamProvider, useStream} from "./UseStream.tsx";
import {DevTools} from "jotai-devtools";
import {finalUrl} from "./baseUrl.ts";
import {Provider, useSetAtom} from "jotai";
import {connectionIdAtom} from "./Atoms.tsx";
import {useEffect} from "react";

// eslint-disable-next-line react-refresh/only-export-components
function ConnectionIdSync() {
    const stream = useStream();
    const setConnectionId = useSetAtom(connectionIdAtom);
    
    useEffect(() => {
        if (stream.connectionId) {
            setConnectionId(stream.connectionId);
        }
    }, [stream.connectionId, setConnectionId]);
    
    return null;
}

createRoot(document.getElementById('root')!).render(
    <Provider>
        <StreamProvider config={{
            connectEvent: 'connected',
            urlForStreamEndpoint: finalUrl+'/api/Telemetry/sse'
        }}>
            <DevTools />
            <ConnectionIdSync />
            <App/>
        </StreamProvider>
    </Provider>,
)