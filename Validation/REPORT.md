# Kết quả kiểm tra

- Unity: 2022.3.62f2 LTS, Windows x64, Mono.
- Biên dịch scene và bản Windows: thành công.
- Luật chơi: **15/15** kiểm tra đạt, gồm mô phỏng xác định 2.000 lượt.
- Thao tác runtime: **22/22** kiểm tra đạt trên bản Windows Development.
- Ảnh chụp: 540 × 960, đã đối chiếu menu, bàn chơi, kéo thả, tạm dừng, hết nước đi và hướng dẫn. Các ảnh kiểm tra có nhãn Development Build; bản Windows giao kèm là bản release.

Chi tiết từng kiểm tra nằm trong `CoreChecks.txt` và `RuntimeChecks.txt`.

Kiểm tra kéo thả gửi sự kiện qua EventSystem tới PieceDragHandler, xác nhận vùng khối nhận con trỏ, đặt đúng ô, cập nhật điểm và tiêu thụ đúng một ô khay. Các kiểm tra còn lại xác nhận tạm dừng chặn thao tác, dùng hết ba khối tạo khay mới, khóa thao tác trong hiệu ứng xóa, phát hiện hết nước đi và bắt đầu ván mới sạch.

Bộ kiểm tra tự phục hồi lựa chọn âm thanh và kỷ lục có sẵn. Các lượt thử không lưu thành kỷ lục của người chơi.

Có thể chạy lại luật chơi từ menu **Block Blast → Validate Core Rules**. Để chạy bộ kiểm tra Windows, dùng Unity batchmode với executeMethod `BlockBlast.Editor.ProjectBuilder.ValidateAndBuildWindows`, sau đó chạy bản vừa build với tham số `--smoke-test`. Sau kiểm tra, dùng **Build Windows Player** để tạo lại bản release.
