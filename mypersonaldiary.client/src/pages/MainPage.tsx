import { Layout, Typography } from "antd";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";

const { Content } = Layout;
const { Title, Paragraph } = Typography;

const MainPage = () => {
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
                    <Title level={2} style={{ textAlign: "center" }}>Test</Title>
                    <Paragraph>
                        Test
                    </Paragraph>
                    <Paragraph>
                        Test
                    </Paragraph>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default MainPage;