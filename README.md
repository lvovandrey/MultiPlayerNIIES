# MultiPlayerNIIES
## Многооконный плеер для синхронного воспроизведения видеофайлов. 
_Player with multi-window interface to play many video files._ 

## Licence
_This repository contains images from Gnome desktop icons pack (https://www.iconspedia.com/pack/gnome-desktop-2042/) with liscense GNU GPL v.2. The rest of this repository is licensed by WTFPL._

## Руководство по эксплуатации

### Основные понятия

##### _Лидер синхронизации_ 

Одно (только одно) окно видеопроигрывателя, которое является основным в процессе синхронизации с другими окнами. 

##### _Титры (SRT-титры, титры с данными о времени)_

Данные, сопровождающие видео, содержащиеся в файле формата \*.srt. Для каждого отдельного титра в файле srt указывается интервал времени, когда он должен отображаться на видео и текст титра (с форматированием - цветом,шрифтом и проч). 

В принципе могут содержать любую текстовую информацию

### 1. Интерфейс

![Общий вид интерфейса](https://github.com/lvovandrey/MultiPlayerNIIES/blob/Readme-Branch/ReadmeImgs/Full2.jpg) 


#### 1.1. Панель управления и таймлайн
![Общий вид интерфейса](https://github.com/lvovandrey/MultiPlayerNIIES/blob/Readme-Branch/ReadmeImgs/TimeLineAndToolPanelNumered.jpg) 

1. Таймлайн (лидера синхронизации)
