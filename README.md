# HIEngine--UI--LoopScrollView
UGUI循环列表

代码源自：[https://github.com/qiankanglai/LoopScrollRect](https://github.com/qiankanglai/LoopScrollRect)
有个人轻微修改，请点击链接查看原文，尊重楼主版权。
下文说明参考自：[https://www.qiankanglai.me/2015/08/15/LoopScrollRect/](https://www.qiankanglai.me/2015/08/15/LoopScrollRect/)

![](https://i.imgur.com/7gw8YSF.gif)

支持ScrollBar，支持横向、纵向及正反向。

![](https://i.imgur.com/ql0v9Lt.gif)

在关闭Mask后可以看到，只有当需要的时候才动态实例化元素，使用完后回收。

![](https://i.imgur.com/64MGQ4f.gif)

##**介绍**

最原始版本的代码是@ivomarel的[InfinityScroll](https://github.com/ivomarel/InfinityScroll)。改到后来，基本和原始版没啥相同的了…

- 原代码使用了sizeDelta作为大小，但是这个在锚点不重合情况下是不成立的
- 支持了GridLayout
- 在启动时检查锚点和轴心，方便使用
- 修复了原代码在往前拖拽会卡顿的问题
- 优化代码，提升性能
- 支持反向滑动
- 支持ScrollBar (在无尽模式下不起作用; 如果元素大小不一致会出现滚动条瑕疵)

**警告**: 为了解决原始代码回拉卡顿的问题，我直接复制了一份UGUI中的ScrollRect代码，而没有继承。这是因为老的做法是在onDrag里停止并立即启动滚动，而我通过修改两个私有变量保证了滑动顺畅。所有我的代码都用==========LoopScrollRect==========这样的注释包起来，维护起来就像打patch了…

##**框架思路**

和UGUI自带的ScrollRect有所不同，我拆分出了LoopHorizontalScrollRect和LoopVerticalScrollRect两个类，分别代表水平滚动条和水平滚动条。下面我们以LoopVerticalScrollRect为例，水平版本类似。

##**判定cell大小**

LoopScrollRect要解决的核心问题是：如何计算每个元素的大小。

这里我**使用了Content Size Fitter配合Layout Element来控制每个cell的长宽，因此对于GridLayout直接取高度，否则取Preferred Height**。需要注意的是，除了元素本身的大小之外，我们还要将padding考虑进去。

    protected override float GetSize(RectTransform item)
    {
	    float size = contentSpacing;
	    if (m_GridLayout != null)
	    {
	    	size += m_GridLayout.cellSize.y;
	    }
	    else
	    {
	    	size += LayoutUtility.GetPreferredHeight(item);
	    }
	    return size;
    }

这个其实也是最核心的一个地方：在能够准确计算格子大小的基础上，后续工作就好实现了。

##如何优雅的增删元素

对于每个ScrollRect，其实只需要考虑在头部和尾部是否需要增加或者删除元素。在这里以头部的各种情况为例进行解释，因为在正向滑动情况下，必须保证在修改元素之后整个ScrollRect内容显示一致不跳变；这些情况比尾部处理会麻烦一些。

NewItemAtStart函数实现了在头部增加一个(或一行，针对GridLayout)元素，并返回这些元素的高度；DeleteItemAtStart代表删除头部的一个元素。**需要注意的是，在修改头部元素之后要及时修改content的anchoredPosition，这样才能保证整个内容区域不会因为多了或者少了一行而产生跳变。**

    protected float NewItemAtStart()
	{
	    float size = 0;
	    for (int i = 0; i < contentConstraintCount; i++)
	    {
	        // Get Element from ObjectPool
	    }
	    if (!reverseDirection)
	    {
	        // Modify content.anchoredPosition
	    }
	    return size;
		}
		protected float DeleteItemAtStart()
		{
		    float size = 0;
		    for (int i = 0; i < contentConstraintCount; i++)
		    {
		        // Return Element to ObjectPool
		    }
		    if (!reverseDirection)
		    {
		        // Modify content.anchoredPosition
		    }
		    return size;
		}

##对象池交互
在新建cell和销毁cell的时候，使用对象池来避免内存碎片；同时这里使用了SendMessage来向每个cell发送必须的信息，保证数据的正确性。

##滚动条相关

这块我其实是估算的，根据当前的长度和当前元素个数/总个数按照比例缩放，这个在所有cell大小一致的情况下是没有问题的；但是如果大小不一致我就无法得到精确结果，所以会产生一定抖动。我暂时没有更好办法，因为得到的信息就是不够用…

##其他细节

我主要遇到了两个坑：



- 增加或者删除元素之后，有时候需要强行调用Canvas.ForceUpdateCanvases()刷新下
- 注意不要在Build Canvas过程中再次修改元素，从而再次触发Build Canvas…

##使用示例

以竖直滚动条为例，介绍一下步骤。如果觉得麻烦的话，直接打开DemoScene复制粘贴就好 =w= 当然你也可以干掉EasyObjPool，自己控制生成和销毁。

- 准备好Prefabs：
- **每个物体上需要贴上Layout Element并指定preferred width/height**
- 如果是正向滑动，就设置pivot为1；否则设为0并打开ReverseDirection。我强烈建议你试试在播放状态下试试看修改这些参数。
- 在Hierarchy里右键，选择UI/Loop Horizontal Scroll Rect或UI/Loop Vertical Scroll Rect即可。使用Component菜单里的也是一样的。

- PrefabSource: 实例化复制的物体
- CachePlace: 多余物体缓存池
- Threshold: 两端预留出来的缓存量(像素数)
- ReverseDirection: 如果是从下往上或者从右往左拖动，就打开这里
- Clear Cells: 清除已有元素，恢复到未初始化状态
- Refill Cells: 初始化并填充元素

![](https://i.imgur.com/iO4y9z9.png)

##无尽模式
如果需要无限滚动模式，将totalCount设为负数即可。