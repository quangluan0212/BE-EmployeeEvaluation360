-- 0. Nếu chưa có thì tạo database
use master
Drop database [VietAn360_DB];
go

IF DB_ID(N'VietAn360_DB') IS NULL
    CREATE DATABASE [VietAn360_DB];
GO

USE [VietAn360_DB];
GO

---------------------------------------------------------------------
-- 1. Bảng CHUCVU
---------------------------------------------------------------------
CREATE TABLE dbo.CHUCVU (
    MaChucVu    INT             IDENTITY(1,1) PRIMARY KEY,
    TenChucVu   VARCHAR(50)     NOT NULL,
    TrangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_CHUCVU_TRANGTHAI DEFAULT('Active')
);
GO

---------------------------------------------------------------------
-- 2. Bảng NGUOIDUNG
---------------------------------------------------------------------
CREATE TABLE dbo.NGUOIDUNG (
    MaNguoiDung     VARCHAR(10)     PRIMARY KEY,
    HoTen           NVARCHAR(100)   NOT NULL,
    Email           VARCHAR(50)     NOT NULL,
    DienThoai       VARCHAR(12)     NULL,
    MatKhau         VARCHAR(60)     NOT NULL,
    NgayVaoCongTy   DATETIME        NULL,
    TrangThai       NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_NGUOIDUNG_TRANGTHAI DEFAULT('Active')
);
GO

---------------------------------------------------------------------
-- 3. Bảng DUAN
---------------------------------------------------------------------
CREATE TABLE dbo.DUAN (
    MaDuAn      INT             IDENTITY(1,1) PRIMARY KEY,
    TenDuAn     NVARCHAR(100)   NOT NULL,
    MoTa        NVARCHAR(255)   NULL,
    TrangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_DUAN_TRANGTHAI DEFAULT('Active')
);
GO

---------------------------------------------------------------------
-- 4. Bảng MAUDANHGIA
---------------------------------------------------------------------
CREATE TABLE dbo.MAUDANHGIA (
    MaMau       INT             IDENTITY(1,1) PRIMARY KEY,
    TenMau      NVARCHAR(255)   NOT NULL,
    LoaiDanhGia NVARCHAR(50)    NOT NULL,
    TrangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_MAUDANHGIA_TRANGTHAI DEFAULT('Active')
);
GO

---------------------------------------------------------------------
-- 5. Bảng DOT_DANHGIA
---------------------------------------------------------------------
CREATE TABLE dbo.DOT_DANHGIA (
    MaDotDanhGia    INT             IDENTITY(1,1) PRIMARY KEY,
    TenDot          NVARCHAR(255)   NOT NULL,
    ThoiGianBatDau  DATETIME        NOT NULL,
    ThoiGianKetThuc DATETIME        NOT NULL,
    TrangThai       NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_DOT_DANHGIA_TRANGTHAI DEFAULT('Chưa bắt đầu')
);
GO

---------------------------------------------------------------------
-- 6. Bảng CHITIET_DOTDANHGIA (junction bảng Dot↔Mau)
---------------------------------------------------------------------
CREATE TABLE dbo.CHITIET_DOTDANHGIA (
    MaDotDanhGia         INT           NOT NULL,
    MaMau                INT           NOT NULL,
    LoaiNguoiDuocDanhGia VARCHAR(50)   NULL,
    CONSTRAINT PK_CHITIET_DOTDANHGIA PRIMARY KEY (MaDotDanhGia, MaMau),
    CONSTRAINT FK_CTDDG_DOT FOREIGN KEY (MaDotDanhGia)
        REFERENCES dbo.DOT_DANHGIA(MaDotDanhGia) ON DELETE NO ACTION,
    CONSTRAINT FK_CTDDG_MAU FOREIGN KEY (MaMau)
        REFERENCES dbo.MAUDANHGIA(MaMau) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 7. Bảng NHOM
---------------------------------------------------------------------
CREATE TABLE dbo.NHOM (
    MaNhom     INT             IDENTITY(1,1) PRIMARY KEY,
    TenNhom    NVARCHAR(100)   NOT NULL,
    MaDuAn     INT             NOT NULL,
    TrangThai  NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_NHOM_TRANGTHAI DEFAULT('Active'),
    CONSTRAINT FK_NHOM_DUAN FOREIGN KEY (MaDuAn)
        REFERENCES dbo.DUAN(MaDuAn) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 8. Bảng NHOM_NGUOIDUNG (junction bảng Nhom↔NguoiDung)
---------------------------------------------------------------------
CREATE TABLE dbo.NHOM_NGUOIDUNG (
    MaNhomNguoiDung INT         IDENTITY(1,1) PRIMARY KEY,
    MaNhom          INT         NOT NULL,
    MaNguoiDung     VARCHAR(10) NOT NULL,
    VaiTro          NVARCHAR(20) NOT NULL,
    TrangThai       NVARCHAR(20) NOT NULL
        CONSTRAINT DF_NHOM_NGUOIDUNG_TRANGTHAI DEFAULT('Active'),
    CONSTRAINT FK_NNGD_NHOM FOREIGN KEY (MaNhom)
        REFERENCES dbo.NHOM(MaNhom) ON DELETE NO ACTION,
    CONSTRAINT FK_NNGD_NGUOIDUNG FOREIGN KEY (MaNguoiDung)
        REFERENCES dbo.NGUOIDUNG(MaNguoiDung) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 9. Bảng NGUOIDUNG_CHUCVU (junction bảng ND↔ChucVu)
---------------------------------------------------------------------
CREATE TABLE dbo.NGUOIDUNG_CHUCVU (
    MaNguoiDung VARCHAR(10)  NOT NULL,
    MaChucVu    INT          NOT NULL,
    CapBac      INT          NOT NULL,
    TrangThai   NVARCHAR(20) NOT NULL
        CONSTRAINT DF_NGUOIDUNG_CHUCVU_TRANGTHAI DEFAULT('Active'),
    CONSTRAINT PK_NGUOIDUNG_CHUCVU PRIMARY KEY (MaNguoiDung, MaChucVu),
    CONSTRAINT FK_NDCV_NGUOIDUNG FOREIGN KEY (MaNguoiDung)
        REFERENCES dbo.NGUOIDUNG(MaNguoiDung) ON DELETE NO ACTION,
    CONSTRAINT FK_NDCV_CHUCVU FOREIGN KEY (MaChucVu)
        REFERENCES dbo.CHUCVU(MaChucVu) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 10. Bảng CAUHOI
---------------------------------------------------------------------
CREATE TABLE dbo.CAUHOI (
    MaCauHoi   INT             IDENTITY(1,1) PRIMARY KEY,
    MaMau      INT             NOT NULL,
    NoiDung    NVARCHAR(500)   NULL,
    DiemToiDa  INT             NULL,
    CONSTRAINT FK_CAUHOI_MAU FOREIGN KEY (MaMau)
        REFERENCES dbo.MAUDANHGIA(MaMau) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 11. Bảng DANHGIA
---------------------------------------------------------------------
CREATE TABLE dbo.DANHGIA (
    MaDanhGia        INT           IDENTITY(1,1) PRIMARY KEY,
    NguoiDanhGia     VARCHAR(10)   NOT NULL,
    NguoiDuocDanhGia INT           NOT NULL,
    MaDotDanhGia     INT           NOT NULL,
    Diem             DECIMAL(5,2)  NOT NULL,
	HeSo			 INT           NOT NULL,
    TrangThai        NVARCHAR(20)  NOT NULL
        CONSTRAINT DF_DANHGIA_TRANGTHAI DEFAULT('Pending'),
    CONSTRAINT FK_DANHGIA_NGUOIDUNG FOREIGN KEY (NguoiDanhGia)
        REFERENCES dbo.NGUOIDUNG(MaNguoiDung) ON DELETE NO ACTION,
    CONSTRAINT FK_DANHGIA_NHOM_NGUOIDUNG FOREIGN KEY (NguoiDuocDanhGia)
        REFERENCES dbo.NHOM_NGUOIDUNG(MaNhomNguoiDung) ON DELETE NO ACTION,
    CONSTRAINT FK_DANHGIA_DOT FOREIGN KEY (MaDotDanhGia)
        REFERENCES dbo.DOT_DANHGIA(MaDotDanhGia) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 12. Bảng DANHGIA_CAUHOI (junction bảng DanhGia↔CauHoi)
---------------------------------------------------------------------
CREATE TABLE dbo.DANHGIA_CAUHOI (
    MaDanhGia INT NOT NULL,
    MaCauHoi  INT NOT NULL,
    TraLoi    INT NULL,
    CONSTRAINT PK_DANHGIA_CAUHOI PRIMARY KEY (MaDanhGia, MaCauHoi),
    CONSTRAINT FK_DGC_DANHGIA FOREIGN KEY (MaDanhGia)
        REFERENCES dbo.DANHGIA(MaDanhGia) ON DELETE NO ACTION,
    CONSTRAINT FK_DGC_CAUHOI FOREIGN KEY (MaCauHoi)
        REFERENCES dbo.CAUHOI(MaCauHoi) ON DELETE NO ACTION
);
GO

---------------------------------------------------------------------
-- 13. Bảng KETQUA_DANHGIA
---------------------------------------------------------------------
CREATE TABLE dbo.KETQUA_DANHGIA (
    MaKetQua      INT           IDENTITY(1,1) PRIMARY KEY,
    MaNguoiDung   VARCHAR(10)   NOT NULL,
    DiemTongKet   DECIMAL(5,2)  NOT NULL,
    ThoiGianTinh  DATETIME      NOT NULL,
    CONSTRAINT FK_KQDG_NGUOIDUNG FOREIGN KEY (MaNguoiDung)
        REFERENCES dbo.NGUOIDUNG(MaNguoiDung) ON DELETE NO ACTION
);

GO

INSERT INTO dbo.CHUCVU (TenChucVu, TrangThai)
VALUES 
    ('Admin', 'Active'),             -- Admin của hệ thống
    ('Leader', 'Active'),                  -- Quản lý dự án
    ('Software Engineer', 'Active'),                -- Kỹ sư phần mềm
    ('Frontend Developer', 'Active'),               -- Lập trình viên Front-end
    ('Backend Developer', 'Active'),                -- Lập trình viên Back-end
    ('Full Stack Developer', 'Active'),             -- Lập trình viên Full Stack
    ('DevOps Engineer', 'Active'),                  -- Kỹ sư DevOps
    ('QA Engineer', 'Active'),                      -- Kỹ sư Kiểm thử chất lượng
    ('Business Analyst', 'Active'),                 -- Chuyên viên phân tích nghiệp vụ
    ('UI/UX Designer', 'Active'),                   -- Thiết kế UI/UX
    ('Data Scientist', 'Active'),                   -- Nhà khoa học dữ liệu
    ('Network Engineer', 'Active'),                 -- Kỹ sư mạng
    ('Security Specialist', 'Active'),              -- Chuyên gia bảo mật
    ('IT Support Specialist', 'Active'),            -- Chuyên viên hỗ trợ IT
    ('Mobile Developer', 'Active'),                 -- Lập trình viên Mobile
    ('Cloud Engineer', 'Active');                   -- Kỹ sư điện toán đám mây
GO

INSERT INTO dbo.DUAN (TenDuAn, MoTa)
VALUES 
(N'Dự án Alpha', N'Dự án phát triển phần mềm quản lý khách hàng'),
(N'Dự án Beta', N'Nâng cấp hệ thống ERP'),
(N'Dự án Gamma', N'Triển khai phần mềm kế toán nội bộ'),
(N'Dự án Delta', N'Ứng dụng theo dõi tiến độ sản xuất'),
(N'Dự án Epsilon', N'Nghiên cứu công nghệ AI cho phân tích dữ liệu'),
(N'Dự án Zeta', N'Thiết kế website thương mại điện tử'),
(N'Dự án Eta', N'Phát triển ứng dụng di động đặt lịch khám bệnh'),
(N'Dự án Theta', N'Tự động hóa quy trình kho vận'),
(N'Dự án Iota', N'Triển khai hệ thống CRM cho khách hàng doanh nghiệp'),
(N'Dự án Kappa', N'Xây dựng dashboard giám sát thời gian thực'),
(N'Dự án Lambda', N'Hệ thống quản lý tài sản cố định'),
(N'Dự án Mu', N'Ứng dụng chatbot chăm sóc khách hàng'),
(N'Dự án Nu', N'Tích hợp dữ liệu từ nhiều hệ thống khác nhau'),
(N'Dự án Xi', N'Phát triển hệ thống chấm công bằng nhận diện khuôn mặt'),
(N'Dự án Omicron', N'Ứng dụng đặt hàng cho nhà phân phối'),
(N'Dự án Pi', N'Nền tảng học trực tuyến cho nội bộ công ty'),
(N'Dự án Rho', N'Phát triển hệ thống kiểm tra chất lượng sản phẩm'),
(N'Dự án Sigma', N'Ứng dụng thống kê và báo cáo tài chính'),
(N'Dự án Tau', N'Triển khai hệ thống quản lý dự án chuẩn PMI'),
(N'Dự án Upsilon', N'Phần mềm đánh giá KPI nhân viên');
