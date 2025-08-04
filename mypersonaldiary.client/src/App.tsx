import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ConfigProvider } from "antd";
import { Toaster } from "sonner";
import AppRoutes from "./routes/AppRoutes";
import theme from "../theme.json";
import { AuthProvider } from "./context/AuthContext";

function App() {
    const queryClient = new QueryClient();

    return (
        <QueryClientProvider client={queryClient}>
            <AuthProvider>
                <ConfigProvider theme={theme}>
                    <Toaster position="top-right" richColors />
                    <AppRoutes />
                </ConfigProvider>
            </AuthProvider>
        </QueryClientProvider>
    );
}

export default App;