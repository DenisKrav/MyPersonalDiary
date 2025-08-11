import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import type { JSX } from "react";
import { UserRole } from "../enums/UserRole";

const AdminRoute = ({ children }: { children: JSX.Element }) => {
    const { isAuthenticated, userRole } = useAuth();

    const isAdmin = userRole === UserRole.Admin;

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (!isAdmin) {
        return <Navigate to="/" replace />;
    };

    return children;
}

export default AdminRoute;
