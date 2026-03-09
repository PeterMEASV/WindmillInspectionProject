import './Dashboard.css'
import {useState} from "react";

function Dashboard() {

    const alarms = 1;
    const turbines = ["Alpha", "Beta", "Charlie", "Delta"]
    const [selectedTurbine, setSelectedTurbine] = useState<string | null>("Select a Turbine");

    const handleTurbineSelect = (turbine: string) => {
        setSelectedTurbine(turbine);
        console.log("Selected turbine:", turbine);
        // Add your logic here
    }

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
            </div>
            <div className="navbar-center">
                <a className="btn btn-ghost text-xl">Mindst 2 Commits</a>
            </div>
            <div className="navbar-end mr-5">
                <button className="btn btn-ghost btn-circle indicator">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor"> <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" /> </svg>

                    {alarms > 0 && (
                        <span className="badge badge-xs badge-primary indicator-item">
                            {alarms >= 100 ? '99+' : alarms}
                        </span>
                    )}
                </button>
            </div>
        </div>
        <div className="screen-content flex flex-wrap gap-4">
            <div className="card w-96 bg-base-200 card-lg shadow-md border-2 border-base-300">
                <div className="card-body">
                    <h2 className="card-title">Large Card</h2>
                    <p>A card component has a figure, a body part, and inside body there are title and actions parts</p>
                    <div className="justify-end card-actions">
                        <button className="btn btn-primary">Buy Now</button>
                    </div>
                </div>
            </div>
            <div className="card w-96 bg-base-200 card-lg shadow-md border-2 border-base-300">
                <div className="card-body">
                    <h2 className="card-title">Large Card</h2>
                    <p>A card component has a figure, a body part, and inside body there are title and actions parts</p>
                    <div className="justify-end card-actions">
                        <button className="btn btn-primary">Buy Now</button>
                    </div>
                </div>
            </div>
            <div className="card w-96 bg-base-200 card-lg shadow-md border-2 border-base-300">
                <div className="card-body">
                    <h2 className="card-title">Large Card</h2>
                    <p>A card component has a figure, a body part, and inside body there are title and actions parts</p>
                    <div className="justify-end card-actions">
                        <button className="btn btn-primary">Buy Now</button>
                    </div>
                </div>
            </div>
        </div>
        </>

    )
}

export default Dashboard