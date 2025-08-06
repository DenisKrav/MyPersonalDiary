import AdminPage from "../pages/AdminPage";
import LogInPage from "../pages/LogInPage";
import NotFoundPage from "../pages/NotFoundPage";
import RegisterPage from "../pages/RegisterPage";
import PrivateRoute from "./PrivateRoute";
import PublicOnlyRoute from "./PublicOnlyRoute";

export const routesList = [
    {
        path: "*",
        element: <NotFoundPage />,
    },
    {
        path: "/admin",
        element: (
            <PrivateRoute>
                <AdminPage />
            </PrivateRoute>
        ),
    },
    {
        path: "/login",
        element: (
            <PublicOnlyRoute>
                <LogInPage />
            </PublicOnlyRoute>
        ),
    },
    {
        path: "/invite",
        element: (
            <PublicOnlyRoute>
                <RegisterPage />
            </PublicOnlyRoute>
        )
    }
];