using Board.DTOs;

namespace Board.Contracts;

public record class BoardUpdateContract(uint Id, BoardUpdateDTO DTO);