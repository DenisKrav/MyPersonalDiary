import AdminPage from "../pages/AdminPage";
import LogInPage from "../pages/LogInPage";
import MainPage from "../pages/MainPage";
import NotFoundPage from "../pages/NotFoundPage";
import RegisterPage from "../pages/RegisterPage";
import AdminRoute from "./AdminRoute";
import PrivateRoute from "./PrivateRoute";
import PublicOnlyRoute from "./PublicOnlyRoute";

export const routesList = [
    {
        path: "*",
        element: <NotFoundPage />,
    },
    {
        path: "/",
        element: (
            <PrivateRoute>
                <MainPage />
            </PrivateRoute>
        ),
    },
    {
        path: "/admin",
        element: (
            <AdminRoute>
                <AdminPage />
            </AdminRoute>
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