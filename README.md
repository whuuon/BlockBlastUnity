# Block Blast 2D — Unity project

Game xếp khối đơn giản.

## Mở và chạy

1. Giải nén project vào một thư mục riêng.
2. Trong Unity Hub, chọn **Add → Add project from disk**, trỏ tới thư mục có `Assets`, `Packages`, `ProjectSettings`.
3. Mở bằng Unity 2022.3.62f2, chờ Unity nhập tài nguyên.
4. Mở `Assets/BlockBlast/Scenes/Main.unity`, bấm **Play**, chọn **CHƠI NGAY**.

## Cách chơi

- Kéo một trong ba khối ở khay vào bàn 8 × 8. Khối phóng lớn khi kéo; vùng đặt hiện màu báo hợp lệ hoặc bị chặn.
- Hàng hoặc cột được lấp đầy sẽ biến mất cùng lúc. Các ô còn lại giữ nguyên vị trí.
- Dùng hết ba khối để nhận bộ mới. Có 23 hình khối; không xoay khối trong ván chơi.
- Mỗi ô đặt được cộng 10 điểm. Xóa hàng/cột được thưởng `100 × số hàng/cột × combo`.
- Các lượt xóa liên tiếp tăng combo; lượt không xóa đưa combo về 0.
- Hết ván khi không khối nào trong khay còn đặt được. Có chơi lại, trang chủ, hướng dẫn và bật/tắt âm thanh.
- Bấm nút **TẠM DỪNG** hoặc **Esc** để dừng/tiếp tục. Rời cửa sổ sẽ tự tạm dừng khi đang nhận thao tác.
- Kỷ lục và lựa chọn âm thanh được lưu bằng PlayerPrefs. Ván đang chơi không được lưu giữa các lần thoát ứng dụng.

## Cấu trúc script

| Thư mục | Trách nhiệm |
|---|---|
| `Scripts/Core` | Tọa độ, hình khối, bàn chơi, kết quả đặt, điểm và trạng thái. Không phụ thuộc giao diện. |
| `Scripts/Data` | ScriptableObject cấu hình bàn, hình khối, sprite và font. |
| `Scripts/Gameplay` | Điều phối ván, tạo khay khối và kiểm tra hết nước đi. |
| `Scripts/Input` | Nhận kéo thả, chuyển vị trí con trỏ sang tọa độ bàn, hoàn trả khối khi hủy. |
| `Scripts/UI` | Hiển thị bàn, khối, khay, điểm, các màn hình và co giãn canvas. |
| `Scripts/Services` | Lưu kỷ lục/cài đặt và phát hiệu ứng âm thanh. |
| `Editor` | Tạo scene, prefab, dữ liệu và build Windows. Không tham gia logic chạy game. |
| `Scripts/Validation`, `Editor/Tests` | Kiểm tra tự động; bộ kiểm tra runtime chỉ chạy khi truyền `--smoke-test`. |

`GameController` nhận yêu cầu từ giao diện → kiểm tra bằng `BoardModel` → cập nhật `ScoreModel` → hiển thị và phát âm thanh. UI không tự sửa luật chơi.

