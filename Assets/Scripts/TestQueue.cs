using UnityEngine;
using System;

public class TestQueue
{
    //단순화를 위해 object 데이터 타입 사용
    private object[] a;
    private int front;
    private int rear;

    public TestQueue(int queueSize = 16)                    //default QueueSize = 16이지만 수정 가능
    {
        a = new object[queueSize];
        front = -1;
        rear = -1;
    }

    public void EndQueue(object data)
    {
        //큐가 가득차 있는지 체크
        if ((rear + 1) % a.Length == front)
        {
            //에러 처리 (또는 배열 확장)
            throw new ApplicationException("Full");
        }
        else
        {
            //비어있는 경우
            if (front == -1)
            {
                front++;
            }

            //데이터 추가
            rear = (rear + 1) % a.Length;
            a[rear] = data;
        }
    }

    public object Dequeue()
    {
        //큐가 비어있는지 체크
        if (front == -1 && rear == -1)
        {
            throw new ApplicationException("Empty");
        }
        else
        {
            //데이터 읽기
            object data = a[front];

            //마지막 요소를 읽은 경우
            if (front == rear)
            {
                //특수값 -1로 설정
                front = -1;
                rear = -1;
            }
            else
            {
                //Front 이동
                front = (front + 1) % a.Length;
            }

            return data;
        }
    }
}
