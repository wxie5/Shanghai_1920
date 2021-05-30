1. 把prefab拖入场景
2. 给场地的3个边界加collider并且放到一个统一的layer中
3. 给boss创建一个单独的layer
4. Inspector里面调数值。att range是攻击范围默认1， bosslayerMask选boss的layer， groundlayerMask选四个边界的layer。dmg Taken是每次被攻击所受到的伤害。 maxjumpCD是2阶段跳抓的CD
5. script里面需要在attack（）中调用角色被攻击的function，update中可以加入死亡后的流程。都有备注