import { Layout, Typography } from "antd";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";

const { Content } = Layout;
const { Title, Paragraph } = Typography;

const AdminPage = () => {
    return (
        <Layout style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: "flex", justifyContent: "center", alignItems: "center", backgroundColor: "#f5f5f5" }}>
                <div
                    style={{
                        maxWidth: "700px",
                        width: "100%",
                        padding: "32px",
                        backgroundColor: "#fff",
                        borderRadius: "12px",
                        boxShadow: "0 4px 12px rgba(0,0,0,0.08)",
                    }}
                >
                    <Title level={2} style={{ textAlign: "center" }}>About the URL Shortener</Title>
                    <Paragraph>
                        This service provides URL shortening by generating a random 6-character code composed of uppercase letters, lowercase letters, and digits.
                        When a user submits a long URL, the system ensures that the same URL hasn't already been shortened. If not, it creates a new short code.
                    </Paragraph>
                    <Paragraph>
                        The short code is guaranteed to be unique by checking the database before saving. If a collision is detected, a new code is generated.
                        Once a unique short code is produced, the original URL, its short version, creation date, and user ID are stored.
                    </Paragraph>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default AdminPage;