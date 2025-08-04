import AdminPage from "../pages/AdminPage";
import NotFoundPage from "../pages/NotFoundPage";

export const routesList = [
    {
        path: "*",
        element: <NotFoundPage />,
    },
    {
        path: "/admin",
        element: <AdminPage />,
    }
];