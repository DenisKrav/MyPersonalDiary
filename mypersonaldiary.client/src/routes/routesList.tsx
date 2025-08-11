import AdminPage from "../pages/AdminPage";
import LogInPage from "../pages/LogInPage";
import NotesPage from "../pages/NotesPage";
import NotFoundPage from "../pages/NotFoundPage";
import RegisterPage from "../pages/RegisterPage";
import RestoreAccountPage from "../pages/RestoreAccountPage";
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
                <NotesPage />
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
    },
    {
        path: "/restore-account",
        element: (
            <PublicOnlyRoute>
                <RestoreAccountPage />
            </PublicOnlyRoute>
        )
    }
];