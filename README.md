# Block Blast 2D — Unity project

Game xếp khối đơn giản, hoàn chỉnh cho bài tập Unity. Giao diện tiếng Việt, dùng ảnh gốc xuất từ file Figma được cung cấp. Unity **2022.3.62f2 LTS**.

## Mở và chạy

1. Giải nén project vào một thư mục riêng.
2. Trong Unity Hub, chọn **Add → Add project from disk**, trỏ tới thư mục có `Assets`, `Packages`, `ProjectSettings`.
3. Mở bằng Unity 2022.3.62f2, chờ Unity nhập tài nguyên.
4. Mở `Assets/BlockBlast/Scenes/Main.unity`, bấm **Play**, chọn **CHƠI NGAY**.

Bản Windows đi kèm chạy bằng `BlockBlast.exe`. Giữ nguyên các thư mục và DLL bên cạnh file EXE. Không cần cài Unity để chạy bản này.

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

Mỗi lớp có file riêng; không có chú thích thừa trong mã nguồn.

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

## Chỉnh sửa trong Unity

- Scene đã có đầy đủ liên kết; không cần tự gắn script hay kéo asset.
- `Assets/BlockBlast/Data/GameConfiguration.asset`: danh sách hình khối sử dụng.
- `Assets/BlockBlast/Data/Shapes`: từng hình có danh sách ô riêng, thuận tiện học và mở rộng.
- `Assets/BlockBlast/Data/Theme.asset`: sprite gốc và font.
- `Assets/BlockBlast/Prefabs`: Cell, PieceTile, DraggablePiece.
- Menu **Block Blast → Build Scene From Figma Assets** dựng lại scene/prefab/dữ liệu ban đầu. Lệnh này thay thế scene được tạo, nên lưu bản riêng trước khi tùy chỉnh.
- Menu **Block Blast → Validate Core Rules** kiểm tra luật chơi.
- Menu **Block Blast → Build Windows Player** xuất bản vào thư mục `BlockBlast-Windows` bên cạnh project.

## Asset và kiểm tra

Xem `ASSET_SOURCES.md` để đối chiếu nguồn Figma và cách cắt sprite. Các PNG gốc được giữ nguyên, không tô lại hay thay bằng hình tạm. Hiệu ứng âm thanh ngắn được tạo riêng cho project.

Kết quả kiểm tra cùng ảnh chụp bản chạy được lưu trong `Validation`. Kiểm tra luật bao gồm biên bàn, chồng ô, xóa hàng/cột đồng thời, combo, hết nước đi và mô phỏng 2.000 lượt. Kiểm tra runtime chạy trên bản Windows, đi qua menu, kéo thả bằng EventSystem, đặt khối, tạm dừng, xóa hàng, game over, chơi lại và hướng dẫn.
