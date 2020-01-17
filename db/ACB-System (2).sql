-- phpMyAdmin SQL Dump
-- version 5.0.0
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 172.17.0.3
-- Thời gian đã tạo: Th1 17, 2020 lúc 01:38 PM
-- Phiên bản máy phục vụ: 8.0.18
-- Phiên bản PHP: 7.4.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `ACB-System`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `customer`
--

CREATE TABLE `customer` (
  `customer_id` int(11) NOT NULL,
  `customer_name` text COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `phone` varchar(20) COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `address` text COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `modified_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isActive` tinyint(1) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

--
-- Đang đổ dữ liệu cho bảng `customer`
--

INSERT INTO `customer` (`customer_id`, `customer_name`, `phone`, `address`, `modified_date`, `isActive`) VALUES
(18, 'Dũng Lê', '0963222471', 'cửa hàng tạp hóa thành tài, 76 tổ 7 ấp 4, tân thành', '2020-01-17 13:35:28', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `product`
--

CREATE TABLE `product` (
  `product_id` int(11) NOT NULL,
  `product_name` text COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `order_price` int(11) NOT NULL DEFAULT '0',
  `sale_price` int(11) NOT NULL DEFAULT '0',
  `model` text COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `inventory` int(11) NOT NULL,
  `warranty` int(11) NOT NULL,
  `modified_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isActive` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `sale_detail`
--

CREATE TABLE `sale_detail` (
  `id` int(11) NOT NULL,
  `so_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `price` int(11) NOT NULL DEFAULT '0',
  `total_amount` int(11) DEFAULT NULL,
  `warranty_start` date NOT NULL,
  `warranty_end` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `sale_header`
--

CREATE TABLE `sale_header` (
  `so_id` int(11) NOT NULL,
  `customer_id` int(11) NOT NULL,
  `total_line` int(11) NOT NULL,
  `sub_total` int(11) NOT NULL DEFAULT '0',
  `discount` double NOT NULL DEFAULT '0',
  `tax` double NOT NULL DEFAULT '0',
  `create_by` varchar(20) COLLATE utf8mb4_vietnamese_ci NOT NULL,
  `modified_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isAtive` tinyint(1) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `customer`
--
ALTER TABLE `customer`
  ADD PRIMARY KEY (`customer_id`),
  ADD KEY `customer_id` (`customer_id`);

--
-- Chỉ mục cho bảng `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`product_id`),
  ADD KEY `product_id` (`product_id`);

--
-- Chỉ mục cho bảng `sale_detail`
--
ALTER TABLE `sale_detail`
  ADD PRIMARY KEY (`id`),
  ADD KEY `sale_detail_ibfk_1` (`so_id`),
  ADD KEY `product_id` (`product_id`);

--
-- Chỉ mục cho bảng `sale_header`
--
ALTER TABLE `sale_header`
  ADD PRIMARY KEY (`so_id`),
  ADD KEY `customer_id` (`customer_id`);

--
-- AUTO_INCREMENT cho các bảng đã đổ
--

--
-- AUTO_INCREMENT cho bảng `customer`
--
ALTER TABLE `customer`
  MODIFY `customer_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT cho bảng `product`
--
ALTER TABLE `product`
  MODIFY `product_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `sale_detail`
--
ALTER TABLE `sale_detail`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT cho bảng `sale_header`
--
ALTER TABLE `sale_header`
  MODIFY `so_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `sale_detail`
--
ALTER TABLE `sale_detail`
  ADD CONSTRAINT `sale_detail_ibfk_1` FOREIGN KEY (`so_id`) REFERENCES `sale_header` (`so_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `sale_detail_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Các ràng buộc cho bảng `sale_header`
--
ALTER TABLE `sale_header`
  ADD CONSTRAINT `sale_header_ibfk_1` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`customer_id`) ON DELETE RESTRICT ON UPDATE RESTRICT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

