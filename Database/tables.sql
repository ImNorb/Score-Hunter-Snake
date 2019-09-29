SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

CREATE TABLE `sb_classic` (
  `id` int(11) NOT NULL,
  `name` varchar(32) COLLATE utf8_general_ci NOT NULL,
  `score` int(11) NOT NULL,
  `date` datetime DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

CREATE TABLE `sb_scorehunt` (
  `id` int(11) NOT NULL,
  `name` text COLLATE utf8_general_ci NOT NULL,
  `score` int(11) NOT NULL,
  `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

ALTER TABLE `sb_classic`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `sb_scorehunt`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `sb_classic`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

ALTER TABLE `sb_scorehunt`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;
