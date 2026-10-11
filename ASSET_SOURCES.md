# Nguồn tài nguyên

File do người dùng cung cấp: [arrow 3D — trang Block crush](https://www.figma.com/design/umtlqQ5KlNQYqodETK4jhC/arrow-3D?node-id=218-2).

Các file PNG trong `Assets/BlockBlast/Art/Figma` được tải bằng chức năng **Download image** của Figma, giữ nguyên dữ liệu ảnh gốc.

| File | Layer/nhóm trong Figma | Node đã đối chiếu | Mã ảnh nguồn |
|---|---|---|---|
| `background.png` | bg 2 / bg 3 | 2052:813 / 2239:1488 | 09b5424fef18fd97afebcc48f3355cbe1b578f31 |
| `logo.png` | block_crash_logo 1 | 426:2 | 0f76bb9f2043b2a0facbf98d31f1c2684aa87f5f |
| `button.png` | Rectangle 3 copy 3 5, Frame 8 | 3557:881 | de53ec6c58322f3f25187b326083a817cf5d6d0a |
| `board.png` | ChatGPT Image 16_25_01 22 thg 5, 2026 7 | 3578:1133 | c3506178fd766b9c8955ced34671b33fe11baabe |
| `block_gold.png` | Layer 8 21, Group 714 | 3578:1104 | 279b9b893b0dc7395bac354e91f4e0ad5276be33 |
| `block_red.png` | Các ô trong Group 702 | 3578:1101 | d254642af8654a860efd4794b36af32149439936 |
| `block_green.png` | Các ô trong Group 701 | 3578:1055 | 1198ccf2b0b91975c31938c243c5aaca39eda385 |
| `empty_cell.png` | image 41 | 3578:1134 | 534ee6008fb697f0f25baa20457368f8f80e5bff |

Unity cắt sprite từ ảnh gốc bằng TextureImporter, không sửa PNG:

- Nền: lấy phần phía trên chưa có khối trang trí, rect `(0,1252,941,700)` theo tọa độ pixel từ góc dưới trái. Phần này được giãn theo cửa sổ.
- Ô trống: lấy một ô tối từ ảnh bàn mẫu `image 41`, rect `(12,576,40,42)`. PNG vẫn chứa toàn bộ ảnh tham chiếu.
- Nút: dùng 9-slice để giữ mép khi thay đổi kích thước.
- Các ô màu và khung neon dùng toàn bộ sprite gốc.

Chữ giao diện và bố cục được dựng bằng Unity UI. Font LegacyRuntime là font đi kèm Unity. Bốn âm thanh đặt khối, xóa, sai vị trí và kết thúc được tổng hợp riêng cho bài tập.

Project này được chuẩn bị để học tập với nguồn tài nguyên người dùng cung cấp; nguồn Figma và quyền sử dụng của tài nguyên vẫn thuộc bên cung cấp.
