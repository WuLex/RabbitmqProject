# RabbitmqProject

## 效果如图：
### docker部署rabbitmq
<table>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/rabbitmq_docker_ps.png"/></td>
    </tr>
</table>



### docker命令
```bash
#启动容器
docker start 5dac4d04d05d

docker ps -a --format "table {{.ID}}   {{.Names}}   {{.Status}}"


#进入rabbitmq所在容器内部
docker exec -it 5dac4d04d05d bash

#同上
docker exec -it 5dac4d04d05d /bin/bash

```
### 执行命令清除队列数据,这种方式不仅会清空消息，还会清空所有配置信息，需要谨慎使用
```bash
#关闭应用
docker exec -it 5dac4d04d05d rabbitmqctl  stop_app
#清除队列中的消息
docker exec -it 5dac4d04d05d rabbitmqctl  reset
#再次启动此应用
docker exec -it 5dac4d04d05d  rabbitmqctl  start_app

## 合并执行
docker exec -it 5dac4d04d05d rabbitmqctl  stop_app && docker exec -it 5dac4d04d05d rabbitmqctl  reset && docker exec -it 5dac4d04d05d  rabbitmqctl  start_app
```

### 根据 queue_name 参数，删除对应的队列
```bash
rabbitmqctl delete_queue queue_name
```

### 进入rabbitmq所在容器内部后,执行相关查看命令
```bash

#查看主机名rabbithost
hostname

#数据存储：
cd /var/lib/rabbitmq/mnesia/rabbit@rabbithost/msg_stores/vhosts/

#消息以键值对的形式存储到文件中，一个虚拟机上的所有队列使用同一块存储，每个节点只有一个。存储分为持久化存储（msg_store_persistent）和短暂存储（msg_store_transient）。
#持久化存储的内容在broker重启后不会丢失，短暂存储的内容在broker重启后丢失。
cd ~/mnesia/rabbit@rabbithost/msg_stores/vhosts/628WB79CIFDYO9LJI6DKMI09L/msg_store_persistent

# 显示
ls -l

#查看RabbitMQ的日志：
cat /var/lib/rabbitmq/mnesia/rabbit@rabbithost/msg_stores/vhosts/628WB79CIFDYO9LJI6DKMI09L/recovery.dets
```


### rabbitmq ui
<table>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/queues_publish_msg.png"/></td>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/api.png"/></td>
    </tr>
    <tr>
           <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/admin_users.png"/></td>
           <td></td>
    </tr>
</table>

### web页面
<table>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/list.png"/></td>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/get%20(2).png"/></td>
    </tr>
</table>
