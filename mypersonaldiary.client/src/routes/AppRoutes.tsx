import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { routesList } from "./routesList";

export default function AppRoutes () {
    return(
        <RouterProvider router={createBrowserRouter(routesList)} />
    )
}