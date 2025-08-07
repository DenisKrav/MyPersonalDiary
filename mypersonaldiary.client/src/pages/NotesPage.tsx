import { Layout, Typography, Table, Button, Space, Popconfirm, Input, DatePicker, type UploadFile, Modal, Radio, Upload } from "antd";
import { PlusOutlined, DeleteOutlined, SearchOutlined } from "@ant-design/icons";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";
import { useState, useMemo } from "react";
import dayjs from "dayjs";
import { toast } from "sonner";

const { Content } = Layout;
const { Title } = Typography;

interface Note {
    id: string;
    content: string;
    createdAt: string;
}

const NotesPage = () => {
    const [notes, setNotes] = useState<Note[]>([
        {
            id: "1",
            content: "Перша нотатка",
            createdAt: new Date().toLocaleString(),
        },
        {
            id: "2",
            content: "Друга нотатка",
            createdAt: new Date().toLocaleString(),
        },
        {
            id: "3",
            content: "Третя нотатка",
            createdAt: new Date().toLocaleString(),
        },
        {
            id: "4",
            content: "Четверта нотатка",
            createdAt: new Date().toLocaleString(),
        },
        {
            id: "5",
            content: "П'ята нотатка",
            createdAt: new Date().toLocaleString(),
        },
        {
            id: "6",
            content: "Шоста нотатка",
            createdAt: new Date().toLocaleString(),
        },
    ]);

    const [searchText, setSearchText] = useState("");
    const [selectedDate, setSelectedDate] = useState<dayjs.Dayjs | null>(null);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [noteType, setNoteType] = useState<"text" | "image">("text");
    const [noteContent, setNoteContent] = useState("");
    const [noteImage, setNoteImage] = useState<UploadFile | null>(null);


    const handleDelete = (id: string) => {
        setNotes((prev) => prev.filter((note) => note.id !== id));
    };

    const filteredNotes = useMemo(() => {
        return notes.filter((note) => {
            const matchContent = note.content.toLowerCase().includes(searchText.toLowerCase());

            const createdAtDate = dayjs(note.createdAt);
            const matchDate = !selectedDate || createdAtDate.isSame(selectedDate, "day");

            return matchContent && matchDate;
        });
    }, [notes, searchText, selectedDate]);

    const columns = [
        {
            title: "№",
            dataIndex: "index",
            render: (_: any, __: any, index: number) => index + 1,
            width: 50,
        },
        {
            title: "Content",
            dataIndex: "content",
        },
        {
            title: "Created At",
            dataIndex: "createdAt",
            width: 200,
        },
        {
            title: "Actions",
            key: "actions",
            width: 100,
            render: (_: any, record: Note) => (
                <Popconfirm title="Delete note?" onConfirm={() => handleDelete(record.id)}>
                    <Button danger icon={<DeleteOutlined />} size="small" />
                </Popconfirm>
            ),
        },
    ];

    const handleSaveNote = () => {
        if (noteType === "text" && !noteContent.trim()) {
            toast.warning("Please enter note content");
            return;
        }

        if (noteType === "image" && !noteImage) {
            toast.warning("Please upload an image");
            return;
        }

        const newNote: Note = {
            id: Date.now().toString(),
            content:
                noteType === "text"
                    ? noteContent
                    : `📷 ${noteImage?.name}`,
            createdAt: new Date().toLocaleString(),
        };

        setNotes((prev) => [...prev, newNote]);
        setNoteContent("");
        setNoteImage(null);
        setNoteType("text");
        setIsModalOpen(false);
    };

    return (
        <Layout style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            <SiteHeader />

            <Content style={{ flex: 1, padding: "24px", backgroundColor: "#f5f5f5" }}>
                <div
                    style={{
                        maxWidth: "90%",
                        margin: "0 auto",
                        backgroundColor: "#fff",
                        padding: "32px",
                        borderRadius: "12px",
                        boxShadow: "0 4px 12px rgba(0,0,0,0.08)",
                    }}
                >
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                        <Space style={{ width: "100%", justifyContent: "space-between" }}>
                            <Title level={3} style={{ margin: 0 }}>
                                Your Notes
                            </Title>
                            <Button type="primary" icon={<PlusOutlined />} onClick={() => setIsModalOpen(true)}>
                                Add Note
                            </Button>
                        </Space>

                        <Space direction="horizontal" style={{ flexWrap: "wrap" }}>
                            <Input
                                placeholder="Search by content"
                                prefix={<SearchOutlined />}
                                allowClear
                                value={searchText}
                                onChange={(e) => setSearchText(e.target.value)}
                                style={{ width: 200 }}
                            />
                            <DatePicker
                                onChange={(value) => setSelectedDate(value)}
                                allowClear
                                placeholder="Filter by date"
                            />
                        </Space>

                        <Table
                            dataSource={filteredNotes}
                            columns={columns}
                            rowKey="id"
                            pagination={{ pageSize: 5 }}
                            bordered
                        />

                        <Modal
                            title="Add New Note"
                            open={isModalOpen}
                            onCancel={() => setIsModalOpen(false)}
                            onOk={handleSaveNote}
                            okText="Save"
                        >
                            <Radio.Group
                                value={noteType}
                                onChange={(e) => setNoteType(e.target.value)}
                                style={{ marginBottom: 16 }}
                            >
                                <Radio.Button value="text">Text</Radio.Button>
                                <Radio.Button value="image">Image</Radio.Button>
                            </Radio.Group>

                            {noteType === "text" ? (
                                <Input.TextArea
                                    rows={4}
                                    value={noteContent}
                                    onChange={(e) => setNoteContent(e.target.value)}
                                    placeholder="Enter your note..."
                                />
                            ) : (
                                <Upload
                                    beforeUpload={() => false}
                                    maxCount={1}
                                    onRemove={() => setNoteImage(null)}
                                    onChange={({ file }) => setNoteImage(file)}
                                    style={{ margin: "0 16px"}}
                                >
                                    <Button>Select Image</Button>
                                </Upload>
                            )}
                        </Modal>
                    </Space>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default NotesPage;
