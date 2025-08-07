import { Layout, Typography, Form, Input, Button, message } from "antd";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";
import type { InviteModel } from "../api/Models/Invete/InviteModel";
import { useInviteUser } from "../api/Queries/Invite/useInviteUser";

const { Content } = Layout;
const { Title } = Typography;

const AdminPage = () => {
    const [form] = Form.useForm();

    const { mutateAsync: registerUser, isPending } = useInviteUser();


    const onFinish = async (values: any) => {
        const invite: InviteModel = {
            email: values.email
        }

        try {
            await registerUser(invite);
            message.success("Invite sent successfully");
            form.resetFields();
        } catch (error: any) {
            console.error(error);
            message.error(error.message || "Sending invite failed");
        }
    };

    return (
        <Layout style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: "flex", justifyContent: "center", alignItems: "center", backgroundColor: "#f5f5f5" }}>
                <div
                    style={{
                        maxWidth: "500px",
                        width: "100%",
                        padding: "32px",
                        backgroundColor: "#fff",
                        borderRadius: "12px",
                        boxShadow: "0 4px 12px rgba(0,0,0,0.08)",
                    }}
                >
                    <Title level={3} style={{ textAlign: "center", marginBottom: "24px" }}>
                        Send an invitation to email
                    </Title>

                    <Form form={form} layout="vertical" onFinish={onFinish}>
                        <Form.Item
                            label="User Email"
                            name="email"
                            rules={[
                                { required: true, message: "Enter user email" },
                                { type: "email", message: "Incorect email format" },
                            ]}
                        >
                            <Input placeholder="email@example.com" />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit" block loading={isPending}>
                                Send an invite
                            </Button>
                        </Form.Item>
                    </Form>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default AdminPage;
