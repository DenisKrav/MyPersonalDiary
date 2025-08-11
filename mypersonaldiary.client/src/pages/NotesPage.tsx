import { Layout, Typography, Table, Button, Space, Popconfirm, Input, DatePicker, Modal, Radio, Upload, Spin } from "antd";
import { PlusOutlined, DeleteOutlined, SearchOutlined } from "@ant-design/icons";
import SiteHeader from "../components/SiteHeader";
import SiteFooter from "../components/SiteFooter";
import { useMemo, useState } from "react";
import dayjs from "dayjs";
import { toast } from "sonner";
import { useGetUserNotes } from "../api/Queries/Note/useGetUserNotes";
import type { NoteModel } from "../api/Models/Note/NoteModel";
import { useAuth } from "../context/AuthContext";
import { useAddNote } from "../api/Queries/Note/useAddNote";
import { NoteType } from "../enums/NoteType";
import { useDeleteNote } from "../api/Queries/Note/useDeleteNote";

const { Content } = Layout;
const { Title } = Typography;

interface TableRow {
    id: string;
    content: string;
    createdAt: string;
    rawCreatedAt: string;
    imageSrc: string | null;
}

const NotesPage = () => {
    const { token } = useAuth();

    const { data, isLoading, isError, error, refetch } = useGetUserNotes(token ?? "");
    const { mutateAsync: addNoteMutation, isPending } = useAddNote(token ?? "");
    const { mutateAsync: deleteNoteMutation, isPending: isDeleting } = useDeleteNote(token ?? "");

    const [searchText, setSearchText] = useState("");
    const [selectedDate, setSelectedDate] = useState<dayjs.Dayjs | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [noteType, setNoteType] = useState<NoteType>(NoteType.Text);
    const [noteContent, setNoteContent] = useState("");
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [previewSrc, setPreviewSrc] = useState<string | null>(null);

    const rows: TableRow[] = useMemo(() => {
        const source: NoteModel[] = data ?? [];
        return source.map(n => {
            const content = n.content ?? "";
            const created = dayjs(n.createdAt);

            const imageSrc =
                n.imageData && n.imageContentType
                    ? `data:${n.imageContentType};base64,${n.imageData}`
                    : null;

            return {
                id: n.id,
                content,
                createdAt: created.format("YYYY-MM-DD HH:mm"),
                rawCreatedAt: n.createdAt,
                imageSrc,
            };
        });
    }, [data]);

    const filteredRows = useMemo(() => {
        return rows.filter(r => {
            const matchContent = r.content.toLowerCase().includes(searchText.toLowerCase());
            const matchDate = !selectedDate || dayjs(r.rawCreatedAt).isSame(selectedDate, "day");
            return matchContent && matchDate;
        });
    }, [rows, searchText, selectedDate]);

    const columns = [
        {
            title: "№",
            dataIndex: "index",
            render: (_: any, __: any, index: number) => index + 1,
            width: 60,
        },
        {
            title: "Note",
            key: "note",
            render: (_: any, record: TableRow) => {
                const hasImage = !!record.imageSrc;
                const hasText = !!record.content?.trim();

                if (!hasImage && !hasText) {
                    return <span style={{ color: "#999" }}>—</span>;
                }

                return (
                    <div style={{ display: "flex", alignItems: "flex-start", gap: 12 }}>
                        {hasImage && (
                            <img
                                src={record.imageSrc!}
                                alt="note"
                                onClick={() => setPreviewSrc(record.imageSrc!)}
                                style={{
                                    width: 120,
                                    height: 90,
                                    objectFit: "cover",
                                    borderRadius: 8,
                                    cursor: "zoom-in",
                                    flex: "0 0 auto",
                                }}
                            />
                        )}

                        {hasText && (
                            <div style={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
                                {record.content}
                            </div>
                        )}
                    </div>
                );
            },
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
            render: (_: any, record: TableRow) => {
                const isOlderThan2Days = dayjs().diff(dayjs(record.rawCreatedAt), "day") > 2;
                if (isOlderThan2Days) return null;

                return (
                    <Popconfirm
                        title="Delete note?"
                        okButtonProps={{ loading: isDeleting }}
                        onConfirm={async () => {
                            try {
                                await deleteNoteMutation({
                                    noteId: record.id,
                                    noteType: record.imageSrc ? NoteType.Image : NoteType.Text,
                                });
                                toast.success("Note deleted");
                                refetch();
                            } catch (e: any) {
                                toast.error(e?.message ?? "Delete failed");
                            }
                        }}
                    >
                        <Button danger icon={<DeleteOutlined />} size="small" loading={isDeleting} />
                    </Popconfirm>
                );
            },
        }
    ];

    const handleSaveNote = async () => {
        try {
            if (noteType === NoteType.Text) {
                if (!noteContent.trim()) {
                    toast.warning("Please enter note content");
                    return;
                }
                await addNoteMutation({ kind: NoteType.Text, content: noteContent.trim() });
            } else {
                if (!imageFile) {
                    toast.warning("Please upload an image");
                    return;
                }
                await addNoteMutation({ kind: NoteType.Image, file: imageFile });
            }

            toast.success("Note added");
            setIsModalOpen(false);
            setNoteContent("");
            setImageFile(null);
            setNoteType(NoteType.Text);
        } catch (e: any) {
            toast.error(e?.message ?? "Add note failed");
        }
    };

    if (!token) {
        return <div style={{ padding: 24 }}>Please log in.</div>;
    }
    if (isLoading) {
        return <div style={{ padding: 24 }}>Loading notes…</div>;
    }
    if (isError) {
        return (
            <div style={{ padding: 24 }}>
                Error: {error?.message}
                <Button style={{ marginLeft: 12 }} onClick={() => refetch()}>Repeat</Button>
            </div>
        );
    }

    return (
        <Layout style={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            <SiteHeader />

            <Content style={{ flex: 1, padding: "24px", backgroundColor: "#f5f5f5" }}>
                <Spin spinning={isLoading || isPending}>
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
                                <Space>
                                    <Button onClick={() => refetch()}>Refresh</Button>
                                    <Button type="primary" icon={<PlusOutlined />} onClick={() => setIsModalOpen(true)}>
                                        Add Note
                                    </Button>
                                </Space>
                            </Space>

                            <Space direction="horizontal" style={{ flexWrap: "wrap" }}>
                                <Input
                                    placeholder="Search by content"
                                    prefix={<SearchOutlined />}
                                    allowClear
                                    value={searchText}
                                    onChange={(e) => setSearchText(e.target.value)}
                                    style={{ width: 240 }}
                                />
                                <DatePicker
                                    onChange={(value) => setSelectedDate(value)}
                                    allowClear
                                    placeholder="Filter by date"
                                />
                            </Space>

                            <Table
                                dataSource={filteredRows}
                                columns={columns}
                                rowKey="id"
                                pagination={{ pageSize: 5 }}
                                bordered
                            />

                            <Modal
                                title="Add New Note"
                                open={isModalOpen}
                                onCancel={() => {
                                    setIsModalOpen(false);
                                    setNoteContent("");
                                    setImageFile(null);
                                    setNoteType(NoteType.Text);
                                }}
                                onOk={handleSaveNote}
                                okText="Save"
                                confirmLoading={isPending}
                                okButtonProps={{
                                    disabled:
                                        (noteType === NoteType.Text && !noteContent.trim()) ||
                                        (noteType === NoteType.Image && !imageFile) ||
                                        isPending,
                                }}
                            >
                                <Radio.Group
                                    value={noteType}
                                    onChange={(e) => setNoteType(e.target.value)}
                                    style={{ marginBottom: 16 }}
                                >
                                    <Radio.Button value={NoteType.Text}>Text</Radio.Button>
                                    <Radio.Button value={NoteType.Image}>Image</Radio.Button>
                                </Radio.Group>

                                {noteType === NoteType.Text ? (
                                    <Input.TextArea
                                        rows={4}
                                        value={noteContent}
                                        onChange={(e) => setNoteContent(e.target.value)}
                                        placeholder="Enter your note..."
                                        maxLength={500}
                                    />
                                ) : (
                                    <Upload
                                        beforeUpload={(file) => {
                                            setImageFile(file);
                                            return false;
                                        }}
                                        onRemove={() => setImageFile(null)}
                                        maxCount={1}
                                        accept="image/*"
                                        style={{ margin: "0 10px" }}
                                    >
                                        <Button>Select Image</Button>
                                    </Upload>
                                )}
                            </Modal>

                            <Modal
                                open={!!previewSrc}
                                footer={null}
                                onCancel={() => setPreviewSrc(null)}
                                width={800}
                            >
                                {previewSrc && (
                                    <img src={previewSrc} alt="full" style={{ width: "100%", display: "block" }} />
                                )}
                            </Modal>
                        </Space>
                    </div>
                </Spin>
            </Content>
            <SiteFooter />
        </Layout>
    );
};

export default NotesPage;
