package main

import (
	"fmt"
	"log"
	"os"
	bc "todolist/basecomand"
	u "todolist/user"
)

// var Enevents = make(map[int]int)
var command string

func main() {
	// var command string
	fmt.Println("Welcome to ToDo List!!!")
	fmt.Println("help - для того чтобы узнать доступные")
	for {
		handCmd(writeCommand())
	}
}

func writeCommand() string {
	fmt.Print("-")
	fmt.Scan(&command)
	return command
}

func handCmd(cmd string) {
	if cmd == "help" {
		bc.Help()
		log.Print("Использование команды help")
	}
	if cmd == "del" {
		fmt.Println("Введите название задачи для удаления:")
		bc.DeleteTasks(writeCommand())
		log.Print("Использование команды del")
	}
	if cmd == "add" {
		Label := u.NewToDo()
		bc.PrintToDo(&Label)
		log.Print("Использование команды add")
	}
	if cmd == "list" {
		bc.List()
		log.Print("Использование команды list")
	}
	if cmd == "done" {
		fmt.Println("Введите название задачи для того чтобы ее завершить:")
		bc.Done(writeCommand())
		log.Print("Использование команды done")
	}
	if cmd == "exit" {
		os.Exit(0)
	}
}
