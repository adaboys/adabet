<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   NumberGenerator.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Helpers\Games;

class NumberGenerator
{
    private $min;
    private $max;
    private $number;

    public function __construct(int $min = 0, int $max = 9999)
    {
        $this->min = $min;
        $this->max = $max;

        return $this;
    }

    
    public function generate(): NumberGenerator
    {
        $this->number = random_int($this->min, $this->max);

        return $this;
    }

    
    public function shift(int $shift): NumberGenerator
    {
        if ($shift > $this->max) {
            $shift = $shift % ($this->max - $this->min + 1);
        }

        $this->number = $this->number + $shift <= $this->max
            ? $this->number + $shift
            : $this->min + ($this->number + $shift - $this->max) - 1;

        return $this;
    }

    public function getNumber(): int
    {
        return $this->number;
    }

    public function setNumber(int $number): NumberGenerator
    {
        if ($number > $this->max) {
            $number = $this->min + $number % ($this->max - $this->min + 1);
        }

        $this->number = $number;

        return $this;
    }

    public function getMin(): int
    {
        return $this->min;
    }

    public function setMin(int $value): NumberGenerator
    {
        $this->min = $value;

        return $this;
    }

    public function getMax(): int
    {
        return $this->max;
    }

    public function setMax(int $value): NumberGenerator
    {
        $this->max = $value;

        return $this;
    }
}
