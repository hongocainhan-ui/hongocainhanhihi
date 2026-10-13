using System;

// =======================
// ENUM
// =======================

enum TrangThaiXe
{
    SanSang,
    DangChoThue,
    DangBaoTri
}

enum LoaiGPLX
{
    KhongCo,
    MoTo,
    OTo
}


// =======================
// EXCEPTION (Câu 7)
// =======================

class ThueXeException : Exception
{
    public ThueXeException(string message) : base(message)
    {
    }
}


// =======================
// LỚP TRỪU TƯỢNG PHƯƠNG TIỆN (Câu 5)
// =======================

abstract class PhuongTien
{
    private string maXe;
    private string hangSanXuat;
    private int namSanXuat;
    private TrangThaiXe trangThai;

    public string MaXe
    {
        get { return maXe; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Mã xe không được rỗng");
            maXe = value;
        }
    }

    public string HangSanXuat
    {
        get { return hangSanXuat; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Hãng sản xuất không được rỗng");
            hangSanXuat = value;
        }
    }

    public int NamSanXuat
    {
        get { return namSanXuat; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Năm sản xuất không hợp lệ");
            namSanXuat = value;
        }
    }

    public TrangThaiXe TrangThai
    {
        get { return trangThai; }
        set { trangThai = value; }
    }

    public abstract decimal TinhTien(
        DateTime nhanXe,
        DateTime henTra,
        DateTime traThucTe,
        double soKm
    );

    public abstract bool KiemTraGPLX(KhachHang khachHang);

    public abstract string TenLoai();
}


// =======================
// XE ĐẠP (Câu 5)
// =======================

class XeDap : PhuongTien
{
    public override decimal TinhTien(
        DateTime nhanXe,
        DateTime henTra,
        DateTime traThucTe,
        double soKm)
    {
        double soGio = (traThucTe - nhanXe).TotalHours;
        int gio = (int)Math.Ceiling(soGio);
        decimal tien = gio * 15000m;

        int soNgay = (int)Math.Ceiling(soGio / 24);
        if (soNgay < 1)
            soNgay = 1;

        decimal gioiHan = soNgay * 100000m;
        if (tien > gioiHan)
            tien = gioiHan;

        return tien;
    }

    public override bool KiemTraGPLX(KhachHang khachHang)
    {
        return true; // Xe đạp không yêu cầu GPLX
    }

    public override string TenLoai()
    {
        return "Xe đạp";
    }
}


// =======================
// XE MÁY (Câu 5)
// =======================

class XeMay : PhuongTien
{
    public string BienSo { get; set; }

    public override decimal TinhTien(
        DateTime nhanXe,
        DateTime henTra,
        DateTime traThucTe,
        double soKm)
    {
        int soNgay = (int)Math.Ceiling((henTra - nhanXe).TotalDays);
        if (soNgay < 1)
            soNgay = 1;

        decimal tien = soNgay * 150000m;

        double gioTre = (traThucTe - henTra).TotalHours;
        if (gioTre > 0)
        {
            int gioTreLamTron = (int)Math.Ceiling(gioTre);
            tien += gioTreLamTron * 30000m;
        }

        return tien;
    }

    public override bool KiemTraGPLX(KhachHang khachHang)
    {
        return khachHang.CoGPLXMoTo();
    }

    public override string TenLoai()
    {
        return "Xe máy";
    }
}


// =======================
// Ô TÔ ĐIỆN (Câu 5)
// =======================

class OToDien : PhuongTien
{
    public string BienSo { get; set; }

    public override decimal TinhTien(
        DateTime nhanXe,
        DateTime henTra,
        DateTime traThucTe,
        double soKm)
    {
        int soNgay = (int)Math.Ceiling((henTra - nhanXe).TotalDays);
        if (soNgay < 1)
            soNgay = 1;

        decimal tien = soNgay * 800000m;
        double kmChoPhep = soNgay * 200;

        if (soKm > kmChoPhep)
        {
            double kmVuot = soKm - kmChoPhep;
            tien += (decimal)kmVuot * 2000m;
        }

        return tien;
    }

    public override bool KiemTraGPLX(KhachHang khachHang)
    {
        return khachHang.CoGPLXOTo();
    }

    public override string TenLoai()
    {
        return "Ô tô điện";
    }
}


// =======================
// LỚP KHÁCH HÀNG (Câu 6)
// =======================

abstract class KhachHang
{
    private string maKhachHang;
    private string hoTen;
    private string soDienThoai;
    protected LoaiGPLX[] danhSachGPLX;

    public KhachHang(params LoaiGPLX[] gplx)
    {
        danhSachGPLX = gplx;
    }

    public string MaKhachHang
    {
        get { return maKhachHang; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Mã khách hàng không hợp lệ");
            maKhachHang = value;
        }
    }

    public string HoTen
    {
        get { return hoTen; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Họ tên không hợp lệ");
            hoTen = value;
        }
    }

    public string SoDienThoai
    {
        get { return soDienThoai; }
        set
        {
            if (value == null || value.Length != 10)
                throw new ArgumentException("Số điện thoại phải có 10 chữ số");
            soDienThoai = value;
        }
    }

    public bool CoGPLXMoTo()
    {
        return CoGPLX(LoaiGPLX.MoTo);
    }

    public bool CoGPLXOTo()
    {
        return CoGPLX(LoaiGPLX.OTo);
    }

    public bool CoGPLX(LoaiGPLX loai)
    {
        foreach (LoaiGPLX x in danhSachGPLX)
        {
            if (x == loai)
                return true;
        }
        return false;
    }

    public abstract decimal GiamGia(decimal tien);

    public abstract string LoaiKhach();
}


// =======================
// KHÁCH VÃNG LAI (Câu 6)
// =======================

class KhachVangLai : KhachHang
{
    public KhachVangLai(params LoaiGPLX[] gplx) : base(gplx) { }

    public override decimal GiamGia(decimal tien)
    {
        return tien; // Không giảm giá
    }

    public override string LoaiKhach()
    {
        return "Vãng lai";
    }
}


// =======================
// KHÁCH THÀNH VIÊN BẠC (Câu 6)
// =======================

class KhachHangBac : KhachHang
{
    public KhachHangBac(params LoaiGPLX[] gplx) : base(gplx) { }

    public override decimal GiamGia(decimal tien)
    {
        return tien * 0.95m; // Giảm 5%
    }

    public override string LoaiKhach()
    {
        return "Thành viên Bạc";
    }
}


// =======================
// KHÁCH THÀNH VIÊN VÀNG (Câu 6)
// =======================

class KhachHangVang : KhachHang
{
    public KhachHangVang(params LoaiGPLX[] gplx) : base(gplx) { }

    public override decimal GiamGia(decimal tien)
    {
        return tien * 0.90m; // Giảm 10%
    }

    public override string LoaiKhach()
    {
        return "Thành viên Vàng";
    }
}


// =======================
// PHIẾU THUÊ (Câu 7)
// =======================

class PhieuThue
{
    public string MaPhieu { get; set; }
    public KhachHang KhachHang { get; set; }
    public PhuongTien PhuongTien { get; set; }

    public DateTime ThoiDiemNhan { get; set; }
    public DateTime ThoiDiemHenTra { get; set; }
    public DateTime ThoiDiemTraThucTe { get; set; }

    public double SoKm { get; set; }

    public decimal TinhTien()
    {
        decimal tienGoc = PhuongTien.TinhTien(
            ThoiDiemNhan,
            ThoiDiemHenTra,
            ThoiDiemTraThucTe,
            SoKm
        );

        return KhachHang.GiamGia(tienGoc);
    }

    public void KiemTraDieuKienThue()
    {
        if (PhuongTien.TrangThai != TrangThaiXe.SanSang)
        {
            throw new ThueXeException("Phương tiện không ở trạng thái Sẵn sàng");
        }

        if (!PhuongTien.KiemTraGPLX(KhachHang))
        {
            throw new ThueXeException("Khách hàng không có GPLX phù hợp");
        }
    }

    public void LapPhieu()
    {
        KiemTraDieuKienThue();
        PhuongTien.TrangThai = TrangThaiXe.DangChoThue;
    }
}


// =======================
// CLASS PROGRAM & KỊCH BẢN TEST (Câu 8)
// =======================

class Program
{
    static void ChayTest(
        string maTC,
        string moTa,
        decimal ketQuaMongDoi,
        Func<decimal> hamTinhTien)
    {
        try
        {
            decimal ketQuaThucTe = hamTinhTien();
            string trangThai = (ketQuaThucTe == ketQuaMongDoi) ? "ĐẠT" : "KHÔNG ĐẠT";

            Console.WriteLine("--------------------------------------");
            Console.WriteLine(maTC);
            Console.WriteLine(moTa);
            Console.WriteLine($"Kết quả thực tế : {ketQuaThucTe:N0} đ");
            Console.WriteLine($"Kết quả mong đợi: {ketQuaMongDoi:N0} đ");
            Console.WriteLine($"=> {trangThai}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(maTC);
            Console.WriteLine(moTa);
            Console.WriteLine("LỖI: " + ex.Message);
        }
    }

    static void ChayTestLoi(
        string maTC,
        string moTa,
        Action hanhDong)
    {
        try
        {
            hanhDong();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(maTC);
            Console.WriteLine(moTa);
            Console.WriteLine("=> KHÔNG ĐẠT");
            Console.WriteLine("Đáng lẽ phải báo lỗi.");
        }
        catch (ThueXeException ex)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(maTC);
            Console.WriteLine(moTa);
            Console.WriteLine("Kết quả thực tế: Từ chối, báo lỗi");
            Console.WriteLine("Báo lỗi: " + ex.Message);
            Console.WriteLine("=> ĐẠT");
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("======================================");
        Console.WriteLine("        HỆ THỐNG QUẢN LÝ DT-RENT      ");
        Console.WriteLine("======================================");

        // TC01
        ChayTest(
            "TC01",
            "Khách vãng lai thuê xe đạp 3 giờ",
            45000m,
            () =>
            {
                KhachHang khach = new KhachVangLai();
                PhuongTien xe = new XeDap
                {
                    MaXe = "XD01",
                    HangSanXuat = "Giant",
                    NamSanXuat = 2024,
                    TrangThai = TrangThaiXe.SanSang
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT01",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 5, 11, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 5, 11, 0, 0)
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC02
        ChayTest(
            "TC02",
            "Khách vãng lai thuê xe đạp 27 giờ",
            145000m,
            () =>
            {
                KhachHang khach = new KhachVangLai();
                PhuongTien xe = new XeDap
                {
                    MaXe = "XD02",
                    HangSanXuat = "Giant",
                    NamSanXuat = 2024,
                    TrangThai = TrangThaiXe.SanSang
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT02",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 6, 11, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 6, 11, 0, 0)
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC03
        ChayTest(
            "TC03",
            "Khách vãng lai thuê xe máy 2 ngày, trả trễ 2 giờ 10 phút",
            390000m,
            () =>
            {
                KhachHang khach = new KhachVangLai(LoaiGPLX.MoTo);
                PhuongTien xe = new XeMay
                {
                    MaXe = "XM01",
                    HangSanXuat = "Honda",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.SanSang,
                    BienSo = "66A1-12345"
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT03",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 7, 8, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 7, 10, 10, 0)
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC04
        ChayTest(
            "TC04",
            "Khách vãng lai thuê ô tô điện 1 ngày, đi 350 km",
            1100000m,
            () =>
            {
                KhachHang khach = new KhachVangLai(LoaiGPLX.OTo);
                PhuongTien xe = new OToDien
                {
                    MaXe = "OT01",
                    HangSanXuat = "VinFast",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.SanSang
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT04",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 6, 8, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 6, 8, 0, 0),
                    SoKm = 350
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC05
        ChayTest(
            "TC05",
            "Như TC03, khách thành viên hạng Vàng",
            351000m,
            () =>
            {
                KhachHang khach = new KhachHangVang(LoaiGPLX.MoTo);
                PhuongTien xe = new XeMay
                {
                    MaXe = "XM02",
                    HangSanXuat = "Honda",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.SanSang,
                    BienSo = "66A1-12345"
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT05",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 7, 8, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 7, 10, 10, 0)
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC06
        ChayTest(
            "TC06",
            "Khách hạng Bạc thuê ô tô điện 2 ngày, đi 380 km",
            1520000m,
            () =>
            {
                KhachHang khach = new KhachHangBac(LoaiGPLX.OTo);
                PhuongTien xe = new OToDien
                {
                    MaXe = "OT02",
                    HangSanXuat = "VinFast",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.SanSang
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT06",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = new DateTime(2026, 10, 5, 8, 0, 0),
                    ThoiDiemHenTra = new DateTime(2026, 10, 7, 8, 0, 0),
                    ThoiDiemTraThucTe = new DateTime(2026, 10, 7, 8, 0, 0),
                    SoKm = 380
                };

                phieu.LapPhieu();
                return phieu.TinhTien();
            }
        );

        // TC07
        ChayTestLoi(
            "TC07",
            "Khách chỉ có GPLX mô tô thuê ô tô điện",
            () =>
            {
                KhachHang khach = new KhachHangBac(LoaiGPLX.MoTo);
                PhuongTien xe = new OToDien
                {
                    MaXe = "OT03",
                    HangSanXuat = "VinFast",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.SanSang
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT07",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = DateTime.Now,
                    ThoiDiemHenTra = DateTime.Now.AddDays(1),
                    ThoiDiemTraThucTe = DateTime.Now.AddDays(1)
                };

                phieu.LapPhieu();
            }
        );

        // TC08
        ChayTestLoi(
            "TC08",
            "Thuê phương tiện đang ở trạng thái Đang bảo trì",
            () =>
            {
                KhachHang khach = new KhachHangBac(LoaiGPLX.MoTo);
                PhuongTien xe = new XeMay
                {
                    MaXe = "XM03",
                    HangSanXuat = "Honda",
                    NamSanXuat = 2025,
                    TrangThai = TrangThaiXe.DangBaoTri,
                    BienSo = "66A1-99999"
                };

                PhieuThue phieu = new PhieuThue
                {
                    MaPhieu = "PT08",
                    KhachHang = khach,
                    PhuongTien = xe,
                    ThoiDiemNhan = DateTime.Now,
                    ThoiDiemHenTra = DateTime.Now.AddDays(1),
                    ThoiDiemTraThucTe = DateTime.Now.AddDays(1)
                };

                phieu.LapPhieu();
            }
        );

        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("          KẾT THÚC KIỂM TRA           ");
        Console.WriteLine("======================================");

        Console.ReadKey();
    }
}