import './Dashboard.css'
import {useState, useEffect} from "react";
import {useStream} from "./UseStream.tsx";
import type {Alert, Telemetry} from "./generated-ts-client.ts";
import {MqttClient, telemetryClient} from "./baseUrl.ts";
import {useAtomValue} from "jotai";
import {connectionIdAtom} from "./Atoms.tsx";
import TelemetryCard from "./Components/TelemetryCard.tsx";
import AlertModal from "./Components/AlertModal.tsx";

function Dashboard() {

    // improve to be scalable. This is just sad man
    const turbines = ["turbine-alpha", "turbine-beta", "turbine-gamma", "turbine-delta"]
    const commands = ["setInterval", "stop", "start", "setPitch"]
    const [selectedTurbine, setSelectedTurbine] = useState<string | null>("Select a Turbine");
    const [selectedCommand, setSelectedCommand] = useState<string | null>("Select a Command");
    const [isInputDisabled, setIsInputDisabled] = useState<boolean>(true);
    const [inputType, setInputType] = useState<string | null>(null);
    const [placeholder, setPlaceholder] = useState<string | null>(null);
    const [inputValue, setInputValue] = useState<string | null>(null);
    const stream = useStream();
    const connectionId = useAtomValue(connectionIdAtom);
    const [telemetries, setTelemetries] = useState<Telemetry[]>([]);
    const [alerts, setAlerts] = useState<Alert[]>([]);

    useEffect(() => {
        if (!connectionId) return;
        telemetryClient.subscribeToAllAlerts(connectionId);

        const unsubscribe = stream.on<Alert>("alerts-all", "alerts-all",
            (dto) => {
                console.log("🚨 Alert received:", dto);
                setAlerts(prevAlerts => [...prevAlerts, { ...dto, resolved: false }]);
            }
        );

        return () => {
            unsubscribe();
        };
    }, [connectionId, stream]);

    const handleResolveAlert = (alertId: string) => {
        setAlerts(prevAlerts => 
            prevAlerts.map(alert => 
                alert.id === alertId ? { ...alert, resolved: true } : alert
            )
        );
    };

    const handleTurbineSelect = (turbine: string) => {
        console.log("Selected turbine:", turbine);

        if (connectionId) {
            telemetryClient.switchGroup(connectionId, turbine, selectedTurbine ?? undefined);
        } else {
            console.warn("Connection ID is not here. How did you get here?");
        }
        
        setSelectedTurbine(turbine);
    }

    const handleCommandSelect = (command: string) => {
        console.log("Selected command:", command);

        setSelectedCommand(command);

        if (command == "start")
        {
            setIsInputDisabled(true);
            setPlaceholder("")
        }
        else
        {
            setIsInputDisabled(false);
        }

        if(command == "setInterval" || command == "setPitch")
        {
            setInputType("number");
            setPlaceholder("Enter number");
        }
        else if(command == "stop")
        {
            setInputType("text")
            setPlaceholder("Enter reason for stopping.");
        }
        
        // this exists to just close the dropdown like 200 lines down
        (document.activeElement as HTMLElement)?.blur();
    }

    const handleSendCommand = () => {
        switch (selectedCommand) {
            case "setInterval":
                if(selectedTurbine != null && inputValue != null)
                    if(Number(inputValue) >= 1 && Number(inputValue) <= 60) {
                        MqttClient.setInterval(selectedTurbine, Number(inputValue));
                        console.log(Number(inputValue));
                    }
                    else
                    {
                        alert("Interval must be between 1 and 60 seconds.");
                    }
                break;

            case "stop":
                if(selectedTurbine != null && inputValue != null) {
                    MqttClient.stopTurbine(selectedTurbine, inputValue);
                }
                else
                {
                    alert("Please enter a reason for stopping.");
                }
                break;

            case "start":
                if(selectedTurbine != null)
                    MqttClient.startTurbine(selectedTurbine);
                break;

            case "setPitch":
                if(selectedTurbine != null && inputValue != null)
                    if(Number(inputValue) >= 0 && Number(inputValue) <= 30) {
                        MqttClient.setBladePitch(selectedTurbine, Number(inputValue));
                        console.log(Number(inputValue));
                    }
                    else {
                        alert("Pitch must be between 0 and 30 degrees.");
                    }
                break;
        }
    }


    // Set up stream listener AFTER selectedTurbine changes
    useEffect(() => {
        if (!selectedTurbine || selectedTurbine === "Select a Turbine") return;

        console.log(`[Dashboard] Setting up listener for: ${selectedTurbine}`);
        
        // Clear old data when turbine changes
        setTelemetries([]);

        const unsubscribe = stream.on<Telemetry>(selectedTurbine, selectedTurbine, (dto) => {
            console.log(`[Dashboard] Received telemetry:`, dto);
            setTelemetries(prev => {
                // Only add if it's actually new (check by ID or timestamp)
                const isDuplicate = prev.some(t => 
                    t.id === dto.id || 
                    (t.timestamp === dto.timestamp && t.turbineid === dto.turbineid)
                );
                
                if (isDuplicate) {
                    console.log(`[Dashboard] Duplicate telemetry ignored:`, dto.id);
                    return prev;
                }
                
                // Add new data and keep last 20
                return [...prev, dto].slice(-50);
            });
        });

        return () => {
            console.log(`[Dashboard] Cleaning up listener for: ${selectedTurbine}`);
            unsubscribe();
        };
    }, [selectedTurbine, stream]);

    const unresolvedCount = alerts.filter(a => !a.resolved).length;

    return (
        <>
        <div className="navbar bg-base-100 shadow-sm">
            <div className="navbar-start">
                <div className="dropdown">
                    <div tabIndex={0} role="button" className="btn btn-ghost px-6 text-lg">
                        {selectedTurbine}
                    </div>
                    <ul
                        tabIndex={-1}
                        className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                        {turbines.map((turbine, index) => (
                            <li key={index}><a onClick={() => handleTurbineSelect(turbine)}>{turbine}</a></li>
                        ))}
                    </ul>
                </div>
                <div className="Command-button">
                    <button className="btn btn-ghost px-6 text-lg" onClick={()=> (document.getElementById('my_modal_1') as HTMLDialogElement)?.showModal()}>
                        Commands
                    </button>
                </div>
            </div>
            <div className="navbar-center">
                <a className="btn btn-ghost text-xl">Mindst 2 Commits</a>
            </div>
            <div className="navbar-end mr-5">
                <button 
                    className="btn btn-ghost btn-circle indicator"
                    onClick={() => (document.getElementById('alert_modal') as HTMLDialogElement)?.showModal()}
                >
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor"> <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" /> </svg>

                    {unresolvedCount > 0 && (
                        <span className="badge badge-xs badge-primary indicator-item">
                            {unresolvedCount >= 100 ? '99+' : unresolvedCount}
                        </span>
                    )}
                </button>
            </div>
        </div>
        <div className="screen-content p-4">
            <div className="w-full text-sm text-gray-500 mb-4">
                Data points: {telemetries.length}
            </div>
            
            <div className="grid grid-cols-2 gap-4">
                <TelemetryCard 
                    title="Windspeed"
                    telemetries={telemetries}
                    dataKey="windspeed"
                    color="rgb(16, 185, 129)"
                    unit=" m/s"
                />
                
                <TelemetryCard 
                    title="Wind Direction"
                    telemetries={telemetries}
                    dataKey="winddirection"
                    color="rgb(249, 115, 22)"
                    unit="°"
                />
                
                <TelemetryCard 
                    title="Ambient Temperature"
                    telemetries={telemetries}
                    dataKey="ambienttemperature"
                    color="rgb(236, 72, 153)"
                    unit="°C"
                />

                <TelemetryCard
                    title="Rotor Speed"
                    telemetries={telemetries}
                    dataKey="rotorspeed"
                    color="rgb(139, 92, 246)"
                    unit=" RPM"
                />

                <TelemetryCard
                    title="Power Output"
                    telemetries={telemetries}
                    dataKey="poweroutput"
                    color="rgb(59, 130, 246)"
                    unit=" kW"
                />

                <TelemetryCard
                    title="Nacelle Direction"
                    telemetries={telemetries}
                    dataKey="nacelledirection"
                    color="rgb(14, 165, 233)"
                    unit="°"
                />

                <TelemetryCard
                    title="Blade Pitch"
                    telemetries={telemetries}
                    dataKey="bladepitch"
                    color="rgb(168, 85, 247)"
                    unit="°"
                />

                <TelemetryCard
                    title="Generator Temperature"
                    telemetries={telemetries}
                    dataKey="generatortemp"
                    color="rgb(234, 179, 8)"
                    unit="°C"
                />

                <TelemetryCard
                    title="Gearbox Temperature"
                    telemetries={telemetries}
                    dataKey="gearboxtemp"
                    color="rgb(239, 68, 68)"
                    unit="°C"
                />

                <TelemetryCard
                    title="Vibration"
                    telemetries={telemetries}
                    dataKey="vibration"
                    color="rgb(6, 182, 212)"
                    unit=" mm/s"
                />
            </div>

            {/* Commands Modal */}
            <dialog id="my_modal_1" className="modal">
                <div className="modal-box h-[300px] flex flex-col">
                    <div className="flex-grow">
                        <h3 className="font-bold text-lg">Commands</h3>
                        <p className="py-4">Select a command from the list.</p>
                        <div className="dropdown dropdown-bottom block">
                            <div tabIndex={0} role="button" className="btn btn-ghost px-3 text-lg">
                                {selectedCommand}
                            </div>
                            <ul
                                tabIndex={-1}
                                className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                                {commands.map((command, index) => (
                                    <li key={index}><a onClick={() => handleCommandSelect(command)}>{command}</a></li>
                                ))}
                            </ul>
                        </div>
                        <input type={inputType ?? undefined} placeholder={placeholder ?? undefined} className="input input-bordered w-full max-w-xs mt-4" disabled={isInputDisabled} value={inputValue ?? undefined} onChange={(e) => setInputValue(e.target.value)} />
                    </div>

                    <div className="modal-action justify-end mt-auto gap-2">
                        <button className="btn px-6 py-3" onClick={() => handleSendCommand()}>
                            Send Command
                        </button>

                        <button className="btn px-6 py-3" onClick={() => (document.getElementById('my_modal_1') as HTMLDialogElement)?.close()}>
                            Close
                        </button>
                    </div>
                </div>
            </dialog>

            {/* Alert Modal */}
            <AlertModal alerts={alerts} onResolveAlert={handleResolveAlert} />
        </div>
        </>

    )
}

export default Dashboard