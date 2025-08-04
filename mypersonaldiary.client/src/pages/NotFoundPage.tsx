import { Layout, Button } from "antd";
import { Link } from "react-router-dom";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";

const { Content } = Layout;

const NotFoundPage = () => {
    return (
        <Layout style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: "flex", justifyContent: "center", alignItems: "center", backgroundColor: "#f5f5f5" }}>
                <div style={{ maxWidth: "400px", width: "100%", padding: "20px", backgroundColor: "#fff", borderRadius: "8px", textAlign: "center" }}>
                    <h1 style={{ fontSize: "48px", color: "#ff4d4f" }}>404</h1>
                    <h2>Page not found</h2>
                    <p>Return to the main page</p>

                    <div style={{ borderTop: "1px solid #ddd", margin: "20px 0" }}></div>

                    <Link to="/">
                        <Button type="primary" style={{ width: "100%" }}>
                            Main Page
                        </Button>
                    </Link>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default NotFoundPage;
