# RabbitmqProject

## 效果如图：
### docker部署rabbitmq
<table>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/rabbitmqimg/rabbitmq_docker_ps.png"/></td>
    </tr>
</table>

### 查看命令
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

#查看RabbitMQ的日：
cat /var/lib/rabbitmq/mnesia/rabbit@rabbithost/msg_stores/vhosts/628WB79CIFDYO9LJI6DKMI09L/recovery.dets
```

### docker命令
```bash
docker start 5dac4d04d05d

docker ps -a --format "table {{.ID}}   {{.Names}}   {{.Status}}"
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
