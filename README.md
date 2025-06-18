WinForm 简易画图工具
这是一个基于 .NET Framework 4.7 开发的轻量级 WinForms 图形绘制应用程序，灵感来源于 Windows 10/11 自带的“画图”工具。本项目旨在通过原生 WinForm 技术实现一个基础但功能完整的绘图软件，适合初学者学习图形界面开发、GDI+ 绘图技术以及事件驱动编程思想,无任何第三方库依赖。

主要功能包括：

鼠标自由绘制直线、矩形、圆形、三角形等基本图形

支持设置线条颜色与粗细

支持撤销（Undo）操作

支持保存和打开绘图文件

支持画布尺寸修改和缩放

支持画布旋转和图形旋转

画直线时按住 Shift 键自带对齐 8 个方向（水平、垂直、45°斜线）

支持全局快捷键 Alt + B 快速进入截图模式（即使不在焦点状态）

![image](https://github.com/user-attachments/assets/d4190762-d97f-4a73-81aa-032881e72d72)

使用方式

1. 下载附件中的 ZIP 文件并解压
2. 将 `DrawKit.dll` 添加为项目引用
3. 在代码中引入命名空间：`using DrawKit;`
4. 按照示例调用方法

示例代码

CanvasForm canvasForm = new CanvasForm();

canvasForm.ShowDialog();




